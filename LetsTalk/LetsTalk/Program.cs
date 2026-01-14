using LetsTalk.Context;
using LetsTalk.Client.ViewModels;
using LetsTalk.Components;
using LetsTalk.Controllers;
using LetsTalk.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.AspNetCore.Mvc;

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

builder.Services.AddScoped<MainLayoutViewModel>();
builder.Services.AddScoped<CounterViewModel>();
builder.Services.AddTransient<ServerViewModel>();

// Api controllers
builder.Services.AddScoped<BackApiEf>();
builder.Services.AddScoped<LetsTalkController>();

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
