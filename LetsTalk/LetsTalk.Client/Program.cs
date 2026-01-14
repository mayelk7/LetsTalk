using LetsTalk.Client.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();

await builder.Build().RunAsync();
