using HomebrewHelper.Source.KNNCloudSingleton;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace HomebrewHelper.Source.DataLoaderSingleton
{
    public class DataLoader : ILoadData
    {
        private readonly IManageKNN knnManager;

        private HttpClient client;
        private bool initialized;

        public DataLoader(NavigationManager navigationManager, IManageKNN knnManager) 
        {
            this.knnManager = knnManager;

            string baseURI = navigationManager.BaseUri;
            client = new HttpClient() { BaseAddress = new Uri(baseURI) };
        }

        public async Task LoadData()
        {
            if (initialized) return; //Guards against double init

            //loads monsters.json
            RawMonsterRecord[] monsters = await client.GetFromJsonAsync<RawMonsterRecord[]>("data/monsters.json");
            if (monsters == null) return;

            knnManager.Clear();
            knnManager.SetWeights(new double[16] {
            1, //hp
            1, //ac
            0.1, 0.1, 0.1, 0.1, 0.1, 0.1, //attributes 
            0.4, 0.4, 0.4, //saves
            0.3, //immunities
            0.3, //weaknesses
            0.3, //resistances
            0.8, //ability count
            1, //average damage of highest damaging attack
        });

            //Map and feed points into knnManager
            foreach (RawMonsterRecord monster in monsters)
            {
                knnManager.AddPoint(new Monster().FromRawMonsterRecord(monster));
            }
            initialized = true;
        }

        //Test function for loading in the test monsters.
        public async Task<List<Monster>> LoadTestData()
        {
            List<Monster> output = new List<Monster>();
            RawMonsterRecord[] monsters = await client.GetFromJsonAsync<RawMonsterRecord[]>("data/test_monsters.json");
            foreach (RawMonsterRecord monster in monsters)
            {
                output.Add(new Monster().FromRawMonsterRecord(monster));
            }
            return output;
        }
    }
}
