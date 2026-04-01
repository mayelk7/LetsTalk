using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Context;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;
using MudBlazor;
using System.Collections.ObjectModel;

namespace LetsTalk.Client.ViewModels;

public partial class MainLayoutViewModel(UserContext userContext, AuthStateService authState) : ObservableObject
{

    // Nécessaire pour afficher @(MainLayoutVm.UserContext.CurrentUser?.Username ?? "invité")
    [ObservableProperty]
    private AuthStateService authState = authState;

    [ObservableProperty]
    private string _search = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    // Utilisation d'ObservableCollection pour que le menu MudBlazor se rafraîchisse seul
    // On l'initialise directement avec l'item Home
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menuItems = new()
    {
        new MenuItem("Home", Icons.Material.Rounded.Chat, "")
    };

    /// <summary>
    /// Vide la liste et remet l'item Home (évite les accumulations d'icônes)
    /// </summary>
    private void ResetMenu()
    {
        MenuItems.Clear();
        MenuItems.Add(new MenuItem("Home", Icons.Material.Rounded.Home, ""));
    }

    public async Task InitAsync()
    {
        // 1. On récupère l'ID via l'Auth ou le Context
        var userId = authState.CurrentUser?.UtilisateurId ?? 1;

        // 2. Si pas d'utilisateur, on s'assure que le menu est clean (juste Home)
        if (userId == null)
        {
            ResetMenu();
            return;
        }

        IsLoading = true;
        try
        {
            var response = await ApiManagerService.MakeGetRequest<List<UserServerDto>>($"/api/user/{userId}/servers");

            if (response is { Success: true } && response.Data != null)
            {
                // 3. NETTOYAGE : On vide avant de remplir pour éviter les doublons au reco
                ResetMenu();

                // 4. Ajout des serveurs depuis l'API
                foreach (var srv in response.Data)
                {
                    MenuItems.Add(new MenuItem(
                        srv.ServerName,
                        Icons.Material.Rounded.Cloud,
                        $"server/{srv.ServerId}"
                    ));
                }

                // 5. Ajout du bouton de création à la toute fin
                MenuItems.Add(new MenuItem("Ajouter un Serveur", Icons.Material.Rounded.Add, "NewServer"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur chargement serveurs: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Se déclenche automatiquement quand la propriété Search change
    partial void OnSearchChanged(string value)
    {
        Console.WriteLine($"Recherche : {value}");
    }
}

public record struct MenuItem(string Title, string Icon, string Href);