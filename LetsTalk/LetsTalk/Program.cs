using LetsTalk.Client.Context;
using LetsTalk.Client.Services;
using LetsTalk.Client.Services.Voice;
using LetsTalk.Client.ViewModels;
using LetsTalk.Components;
using LetsTalk.Context;
using LetsTalk.Controllers;
using LetsTalk.Data;
using LetsTalk.Models.Config;
using LetsTalk.Services.Authentication;
using LetsTalk.Services.Authentification;
using LetsTalk.Services.Email;
using LetsTalk.Services.Livekit;
using LetsTalk.Shared.Service;
using LetsTalk.Shared.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMudServices();
builder.Services.AddScoped<AuthStateService>();


builder.Services.AddScoped<HttpClient>(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7235/") 
    };
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
        )
    );
builder.Services.AddScoped<AuthService>();
builder.Services.Configure<LivekitSettings>(
builder.Configuration.GetSection("LivekitSettings"));
builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<LoginLayoutViewModel>();
builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();
builder.Services.AddTransient<TextChannelViewModel>();

// Lie le JSON à la classe EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Enregistre le service d'email
builder.Services.AddScoped<IEmailService, EmailService>();

// Api controllers
builder.Services.AddScoped<BackApiEf>();
builder.Services.AddScoped<LetsTalkController>();
builder.Services.AddScoped<ServerApiController>();
builder.Services.AddScoped<UserApiController>();

/*
builder.Services.AddScoped<VoiceServer>();
// Voice services
// 1. Enregistrer VoiceServer avec AddTransient/AddSingleton. 
// �tant un service unique, AddSingleton est le plus appropri�.
builder.Services.AddSingleton<VoiceServer>();

// 2. Enregistrer VoiceServer comme Service H�berg� pour qu'il d�marre et s'arr�te 
// automatiquement avec l'application web.
builder.Services.AddHostedService<VoiceServerHost>();

*/
// Enregistrement de votre LivekitService (Singleton est parfait ici)
builder.Services.AddSingleton<LivekitService>();
builder.Services.AddSingleton<LiveKitServiceClient>();

// 3. Enregistrement du service h�berg� pour l'ex�cution au d�marrage
builder.Services.AddHostedService<LivekitInitializer>();

builder.Services.AddSingleton<UserContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    })
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
