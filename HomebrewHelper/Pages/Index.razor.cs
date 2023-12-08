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

        //Called during initialization, after page renders
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            { 
                await dataLoader.LoadData();

                //Switch this to true to run analysis and tests on startup
                //WARNING: Takes a while
                if (false)
                {
                    List<Monster> testMonsters = await dataLoader.LoadTestData();
                    TestNeighborhoodSizes(testMonsters);
                    TestWeights(testMonsters);
                }

                //Hides loading screen to allow access to app
                loaderHidden = "hidden";
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        //Called when changes are made to inputs to update the estimated level and neighbors display
        protected void RefreshQuery() 
        {
            int?[] queryInputs = new int?[16];
            inputs.CopyTo(queryInputs, 0);
            if (damageComponents[0] != null && damageComponents[1] != null) 
            {
                //Computes the average damage of an attack based on the number and value of the dice roll, and its flat bonus
                //The format for this xdy + z is very common in tabletop games for defining damage
                queryInputs[15] = (damageComponents[0] + (damageComponents[0] * damageComponents[1]) / 2) + (damageComponents[2] != null ? damageComponents[2] : 0);
            }

            level = "?";
            foreach (int? input in queryInputs) 
            {
                //This loop-if structure exists to ensure that level is only estimated if there is at least 1 valid input.
                if (input != null) 
                {
                    level = knnManager.EstimateLevel(queryInputs, 22).ToString();
                    monsters = knnManager.GetNearestNeighbors(queryInputs, 8);
                    StateHasChanged();
                    break;
                }
            }            
        }

        //Callback function for when a new monster is selected, to trigger UI change
        public void ChangeSelectedMonster(Monster monster) 
        {
            selectedMonster = monster;
            StateHasChanged();
        }

        //Tests different weighting profiles and prints stats on them to the console
        private void TestWeights(List<Monster> testMonsters)
        {
            List<double[]> weights = new List<double[]>() {
                new double[16] { 1, 1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.4, 0.4, 0.4, 0.3, 0.3, 0.3, 0.8, 1 }, //balanced-defensive
                new double[16] { 1, 1, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.3, 0.3, 0.3, 0.3, 0.3, 0.3, 1, 1 }, //default
                new double[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, //flat
                new double[16] { 1, 1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.4, 0.4, 0.4, 0.4, 0.4, 0.2, 0.5, 0.5 }, //favor defense
                new double[16] { 0.5, 0.5, 0.3, 0.3, 0.3, 0.3, 0.3, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 1, 1 }, //favor offense
                new double[16] { 1, 1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 1, 1 }, //hp-ac-dmg
            };
            foreach (double[] weight in weights)
            {
                int correct = 0;
                int incorrect = 0;
                int predictionDistance = 0;

                knnManager.SetWeights(weight);

                foreach (Monster monster in testMonsters)
                {
                    int prediction = knnManager.EstimateLevel(monster.Hyperposition, 22);
                    if (prediction == monster.Level)
                    {
                        correct++;
                    }
                    else
                    {
                        incorrect++;
                        predictionDistance += Math.Abs(prediction - monster.Level);
                    }
                }

                Console.WriteLine($"Ran Startup Test for weights; Correct Predictions: {correct}; Incorrect Predictions: {incorrect}; Prediction Distance: {predictionDistance}; Accuracy: {(float)correct / (correct + incorrect)}; Average Misprediction: {(float)predictionDistance / (incorrect > 0 ? incorrect : 1)}");
            }

            //Reset weights to default for real app.
            knnManager.SetWeights(weights[0]);
        }

        //Tests neighborhood sizes between 1 and 40 and prints stats about each neighborhood size's performance to the console
        private void TestNeighborhoodSizes(List<Monster> testMonsters)
        {
            for (int i = 1; i <= 40; i += 4)
            {
                int correct = 0;
                int incorrect = 0;
                int predictionDistance = 0;

                foreach (Monster monster in testMonsters)
                {
                    int prediction = knnManager.EstimateLevel(monster.Hyperposition, i);
                    if (prediction == monster.Level)
                    {
                        correct++;
                    }
                    else
                    {
                        incorrect++;
                        predictionDistance += Math.Abs(prediction - monster.Level);
                    }
                }

                Console.WriteLine($"Ran Startup Test on Neighborhood ({i}); Correct Predictions: {correct}; Incorrect Predictions: {incorrect}; Prediction Distance: {predictionDistance}; Accuracy: {(float)correct / (correct + incorrect)}; Average Misprediction: {(float)predictionDistance / (incorrect > 0 ? incorrect : 1)}");
            }
        }
    }
}
