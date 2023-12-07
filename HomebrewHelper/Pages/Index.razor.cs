using HomebrewHelper.Shared;
using HomebrewHelper.Source;
using HomebrewHelper.Source.DataLoaderSingleton;
using HomebrewHelper.Source.KNNCloudSingleton;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace HomebrewHelper.Pages
{
    public partial class Index
    {
        [Inject]
        public ILoadData dataLoader { get; set; }

        [Inject]
        public IManageKNN knnManager { get; set; }

        protected int?[] inputs = new int?[15];
        protected int?[] damageComponents = new int?[3];
        protected string level = "?";
        protected List<Monster> monsters = new List<Monster>();
        protected Monster selectedMonster;
        protected string loaderHidden = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            { 
                await dataLoader.LoadData();
                loaderHidden = "hidden";
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        protected override async Task OnInitializedAsync()
        {
            
            await base.OnInitializedAsync();
        }

        protected void RefreshQuery() 
        {
            int?[] queryInputs = new int?[16];
            inputs.CopyTo(queryInputs, 0);
            if (damageComponents[0] != null && damageComponents[1] != null) 
            {
                queryInputs[15] = (damageComponents[0] + (damageComponents[0] * damageComponents[1])) + (damageComponents[2] != null ? damageComponents[2] : 0);
            }

            level = "?";
            foreach (int? input in queryInputs) 
            {
                if (input != null) 
                {
                    level = knnManager.EstimateLevel(queryInputs, 5).ToString();
                    monsters = knnManager.GetNearestNeighbors(queryInputs, 7);
                    StateHasChanged();
                    break;
                }
            }            
        }

        public void ChangeSelectedMonster(Monster monster) 
        {
            selectedMonster = monster;
            StateHasChanged();
        }
    }
}
