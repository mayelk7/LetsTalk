using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Context;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;
using MudBlazor;

namespace LetsTalk.Client.ViewModels;

public partial class MainLayoutViewModel(UserContext userContext) : ObservableObject
{
    [ObservableProperty]
    private UserContext _userContext = userContext;
    
    [ObservableProperty]
    private List<MenuItem> _menuItems = [
        new() { Title = "Home", Icon = Icons.Material.Rounded.Chat, Href = "" }
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
        //TODO: Temp user injection
        this.UserContext.CurrentUser = new UserDto(
            1,
            "CurrentUser", // TODO: Replace with actual current user
            "john.doe@exemple.com",
            "1234567890",
            null,
            DateTime.Now
        );
        
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
        //ajouter le bouton add en dernier
        MenuItems.Add(new MenuItem("Ajouter un Server", Icons.Material.Rounded.Add, "NewServer"));
    }
}

public record struct MenuItem (string Title, string Icon, string Href);
