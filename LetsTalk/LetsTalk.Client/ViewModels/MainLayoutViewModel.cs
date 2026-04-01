using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Context;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.ObjectModel;

namespace LetsTalk.Client.ViewModels;

public partial class MainLayoutViewModel : ObservableObject
{
    [ObservableProperty]
    private UserContext _userContext;

    // Utilisation d'ObservableCollection pour que l'UI (MudNavMenu) 
    // se mette à jour dès qu'on ajoute un serveur
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menuItems = new()
    {
        new MenuItem("Accueil", Icons.Material.Rounded.Chat, "")
    };

    [ObservableProperty]
    private string _search = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public MainLayoutViewModel(UserContext userContext)
    {
        _userContext = userContext;

        // On écoute les changements internes du UserContext.
        // Si CurrentUser change, on notifie l'UI que UserContext a été mis à jour.
        _userContext.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(UserContext.CurrentUser))
            {
                OnPropertyChanged(nameof(UserContext));
                // Optionnel : Recharger les serveurs automatiquement au login
                if (_userContext.IsConnected) _ = InitAsync();
            }
        };
    }

    /// <summary>
    /// Initialise les données du Layout (Serveurs de l'utilisateur)
    /// </summary>
    public async Task InitAsync()
    {
        // Ne pas charger si l'utilisateur n'est pas connecté 
        // ou si les serveurs sont déjà là
        if (!_userContext.IsConnected || MenuItems.Count > 1)
            return;

        IsLoading = true;
        try
        {
            var userId = _userContext.CurrentUser!.Id;
            var response = await ApiManagerService.MakeGetRequest<List<UserServerDto>>($"/api/user/{userId}/servers");

            if (response is { Success: true } && response.Data != null)
            {
                // On garde uniquement le premier item (Accueil)
                var homeItem = MenuItems[0];
                MenuItems.Clear();
                MenuItems.Add(homeItem);

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
                foreach (var srv in response.Data)
                {
                    MenuItems.Add(new MenuItem(
                        srv.ServerName,
                        Icons.Material.Rounded.Cloud,
                        $"server/{srv.ServerId}"
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des serveurs: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Logique de déconnexion appelée depuis la Navbar
    /// </summary>
    public async Task LogoutAsync(AuthStateService authService, NavigationManager navigation)
    {
        await authService.LogoutAsync();

        // On réinitialise le menu pour le prochain utilisateur
        var homeItem = MenuItems[0];
        MenuItems.Clear();
        MenuItems.Add(homeItem);

        navigation.NavigateTo("/login");
    }

    partial void OnSearchChanged(string value)
    {
        // Logique de recherche globale ici
        Console.WriteLine($"Recherche en cours : {value}");
    }
}

public record struct MenuItem(string Title, string Icon, string Href);