using LetsTalk.Client.ViewModels;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using LetsTalk.Shared.Service;
using UserContext = LetsTalk.Client.Context.UserContext;
using LetsTalk.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<LoginLayoutViewModel>();
builder.Services.AddTransient<TextChannelViewModel>();

builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();
builder.Services.AddScoped<ServerViewModel>();
builder.Services.AddSingleton<LiveKitServiceClient>();
builder.Services.AddSingleton<UserContext>();

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

await builder.Build().RunAsync();
