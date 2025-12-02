using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;

namespace LetsTalk.Client.ViewModels;

public partial class MainLayoutViewModel : ObservableObject
{
    // TODO: Implement dynamic menu item loading based on user server data
    [ObservableProperty]
    private List<MenuItem>? _menuItems;

    [ObservableProperty]
    private string _search;
    
    public MainLayoutViewModel()
    {
        Search = string.Empty;
        
        MenuItems = new List<MenuItem>
        {
            new() { Title = "Home", Icon = Icons.Material.Rounded.Home, Href = "" },
            new() { Title = "Server1", Icon = Icons.Material.Rounded.Cloud, Href = "server/1"},
            new() { Title = "Server2", Icon = Icons.Material.Rounded.CloudSync, Href = "server/2"},
        };
    }

    // TODO: Exemple of reacting to property changes
    partial void OnSearchChanged(string value)
    {
        Console.WriteLine(value);
        // Search = "test";
    }
}

// TODO: Temporary static menu items until dynamic loading is implemented
public record struct MenuItem (string Title, string Icon, string Href);
