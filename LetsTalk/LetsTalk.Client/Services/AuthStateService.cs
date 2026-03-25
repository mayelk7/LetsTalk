using LetsTalk.Shared.ModelsDto;
using Microsoft.JSInterop;
using System.Text.Json;

namespace LetsTalk.Client.Services;

public class AuthStateService
{
    private UserAuthDto? _currentUser;
    private readonly IJSRuntime _jsRuntime;
    private const string LocalStorageKey = "letsTalk_currentUser";

    // Event pour notifier les composants des changements d'état
    public event Action? OnAuthStateChanged;

    public AuthStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }


    /// Obtenir l'utilisateur actuellement connecté

    public UserAuthDto? CurrentUser => _currentUser;


    /// Vérifier si un utilisateur est connecté

    public bool IsAuthenticated => _currentUser != null;


    /// Initialiser le service (charger depuis LocalStorage)
    /// À appeler au démarrage de l'application

    public async Task InitializeAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", LocalStorageKey);

            if (!string.IsNullOrEmpty(json))
            {
                _currentUser = JsonSerializer.Deserialize<UserAuthDto>(json);
                NotifyAuthStateChanged();
            }
        }
        catch (Exception)
        {
            // Si erreur (ex: pas de LocalStorage), ignorer silencieusement
            _currentUser = null;
        }
    }

    /// <summary>
    /// Connecter un utilisateur
    /// </summary>
    public async Task LoginAsync(UserAuthDto user)
    {
        _currentUser = user;
        await SaveToLocalStorageAsync();
        NotifyAuthStateChanged();
    }

    /// <summary>
    /// Déconnecter l'utilisateur
    /// </summary>
    public async Task LogoutAsync()
    {
        _currentUser = null;
        await ClearLocalStorageAsync();
        NotifyAuthStateChanged();
    }

    /// <summary>
    /// Sauvegarder dans LocalStorage
    /// </summary>
    private async Task SaveToLocalStorageAsync()
    {
        try
        {
            if (_currentUser != null)
            {
                var json = JsonSerializer.Serialize(_currentUser);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
            }
        }
        catch (Exception)
        {
            // Ignorer les erreurs de LocalStorage
        }
    }

    /// <summary>
    /// Supprimer de LocalStorage
    /// </summary>
    private async Task ClearLocalStorageAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKey);
        }
        catch (Exception)
        {
            // Ignorer les erreurs
        }
    }

    /// <summary>
    /// Notifier tous les composants abonnés que l'état a changé
    /// </summary>
    private void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }
}