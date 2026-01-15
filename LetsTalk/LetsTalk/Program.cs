using LetsTalk.Client.Services.Voice;
using LetsTalk.Shared.Service;
using LetsTalk.Client.ViewModels;
using LetsTalk.Components;
using LetsTalk.Context;
using LetsTalk.Services.Livekit;
using LetsTalk.Shared.Service;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Controllers;
using LetsTalk.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMudServices();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
        )
    );
builder.Services.Configure<LivekitSettings>(
    builder.Configuration.GetSection("LivekitSettings"));
builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();

// Api controllers
builder.Services.AddScoped<BackApiEf>();
builder.Services.AddScoped<LetsTalkController>();
builder.Services.AddScoped<ServerApiController>();
builder.Services.AddScoped<UserApiController>();

/*
builder.Services.AddScoped<VoiceServer>();
// Voice services
// 1. Enregistrer VoiceServer avec AddTransient/AddSingleton. 
// Étant un service unique, AddSingleton est le plus approprié.
builder.Services.AddSingleton<VoiceServer>();

// 2. Enregistrer VoiceServer comme Service Hébergé pour qu'il démarre et s'arrête 
// automatiquement avec l'application web.
builder.Services.AddHostedService<VoiceServerHost>();

*/
// Enregistrement de votre LivekitService (Singleton est parfait ici)
builder.Services.AddSingleton<LivekitService>();
builder.Services.AddSingleton<LiveKitServiceClient>();

// 3. Enregistrement du service hébergé pour l'exécution au démarrage
builder.Services.AddHostedService<LivekitInitializer>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LetsTalk API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LetsTalk.Client._Imports).Assembly);

app.MapFallbackToFile("index.html");

app.Run();
