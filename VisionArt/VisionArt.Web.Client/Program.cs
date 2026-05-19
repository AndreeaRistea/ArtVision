using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VisionArt.Web.Client.Helpers;
using VisionArt.Web.Client.IServices;
using VisionArt.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiUrl = builder.Configuration["Api:BaseUrl"]
    ?? "https://localhost:7275/api/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ArtApiService>();

await builder.Build().RunAsync();
