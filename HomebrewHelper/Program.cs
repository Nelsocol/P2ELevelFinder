using HomebrewHelper;
using HomebrewHelper.Source.DataLoaderSingleton;
using HomebrewHelper.Source.KNNCloudSingleton;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//Declare custom singleton services
builder.Services.AddSingleton<ILoadData, DataLoader>();
builder.Services.AddSingleton<IManageKNN, KNNManager>();

await builder.Build().RunAsync();
