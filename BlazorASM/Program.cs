using Blazored.LocalStorage;
using ASM.Client.Services;
using BlazorASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7032/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
