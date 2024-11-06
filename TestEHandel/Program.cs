using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TestEHandel;
using TestEHandel.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ProduktService>();

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AuthenticationStateProvider, MockAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
