using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyPortfolio;
using MyPortfolio.Themes;
using MudBlazor;
using MyPortfolio.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config =>
{
	config.SnackbarConfiguration.MaxDisplayedSnackbars = 5;
	config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

builder.Services.AddSingleton<MudTheme, CustomTheme>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ContentService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

await builder.Build().RunAsync();