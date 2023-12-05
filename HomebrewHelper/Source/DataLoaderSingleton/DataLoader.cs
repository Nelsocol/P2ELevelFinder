using HomebrewHelper.Source.KNNCloudSingleton;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace HomebrewHelper.Source.DataLoaderSingleton
{
    public class DataLoader : ILoadData
    {
        private readonly NavigationManager navigationManager;
        private readonly IManageKNN knnManager;

        private HttpClient client;

        public DataLoader(NavigationManager navigationManager, IManageKNN knnManager) 
        {
            this.navigationManager = navigationManager;
            this.knnManager = knnManager;

            string baseURI = navigationManager.BaseUri;
            client = new HttpClient() { BaseAddress = new Uri(baseURI) };
        }

        public async void LoadData()
        {
            RawMonsterRecord[] monsters = await client.GetFromJsonAsync<RawMonsterRecord[]>("data/monsters.json");
            if (monsters == null) return;

            knnManager.Clear();
            knnManager.SetWeights(new double[16] { 
                1, //hp
                1, //ac
                0.2, 0.2, 0.2, 0.2, 0.2, 0.2, //attributes 
                0.3, 0.3, 0.3, //saves
                0.3, //immunities
                0.3, //weaknesses
                0.3, //resistances
                1, //ability count
                1, //average damage of highest damaging attack
            });
            foreach (RawMonsterRecord monster in monsters) 
            {
                knnManager.AddPoint(new Monster().FromRawMonsterRecord(monster));
            }

            var testLevel = knnManager.EstimateLevel(new int?[16] { 240, 34, 8, -1, 4, -5, 0, -5, 26, 21, 22, 16, 0, 1, 9, 29 }, 5);
            Console.WriteLine(testLevel);
        }
    }
}
