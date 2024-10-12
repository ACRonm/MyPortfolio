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

builder.Services.AddMudServices();
builder.Services.AddSingleton<MudTheme, CustomTheme>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.Configure<GitHubSettings>(options => builder.Configuration.GetSection("GitHub").Bind(options));
builder.Services.AddScoped<ContentService>();


var movieApiKey = builder.Configuration["GITHUB_TOKEN"];

await builder.Build().RunAsync();

public class GitHubSettings
{
    public string? Token { get; set; }
}