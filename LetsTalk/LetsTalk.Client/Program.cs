using LetsTalk.Client.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using LetsTalk.Shared.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();
builder.Services.AddScoped<ServerViewModel>();
builder.Services.AddSingleton<LiveKitServiceClient>();

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

await builder.Build().RunAsync();
