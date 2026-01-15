using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;
using MudBlazor;

namespace LetsTalk.Client.ViewModels;

public partial class MainLayoutViewModel : ObservableObject
{
    [ObservableProperty]
    private List<MenuItem> _menuItems = [
        new() { Title = "Home", Icon = Icons.Material.Rounded.Home, Href = "" }
    ];

    [ObservableProperty]
    private string _search = string.Empty;

    // TODO: Exemple of reacting to property changes
    partial void OnSearchChanged(string value)
    {
        Console.WriteLine(value);
        // Search = "test";
    }
    
    public async Task InitAsync()
    {
        var response = await ApiManagerService.MakeGetRequest<List<UserServerDto>>("/api/user/1/servers");

        if (response is { Success: false })
        {
            Console.WriteLine("Error fetching user servers: " + response?.Message);
            return;
        }

        var userServerDtos = response?.Data;

        foreach (var userServerDto in userServerDtos ?? Enumerable.Empty<UserServerDto>())
        {
            MenuItems.Add(new MenuItem(
                    userServerDto.ServerName,
                    Icons.Material.Rounded.Cloud,
                    "server/" + userServerDto.ServerId
                )
            );
        }
    }
}

public record struct MenuItem (string Title, string Icon, string Href);
