using LetsTalk.Shared.ModelsDto;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.Services;

public class AuthStateService
{
    private UserAuthDto? _currentUser;

    // Event pour notifier les composants des changements d'état
    public event Action? OnAuthStateChanged;

    /// <summary>
    /// Obtenir l'utilisateur actuellement connecté
    /// </summary>
    public UserAuthDto? CurrentUser => _currentUser;

    /// <summary>
    /// Vérifier si un utilisateur est connecté
    /// </summary>
    public bool IsAuthenticated => _currentUser != null;

    /// <summary>
    /// Connecter un utilisateur
    /// </summary>
    public void Login(UserAuthDto user)
    {
        _currentUser = user;
        NotifyAuthStateChanged();
    }

    /// <summary>
    /// Déconnecter l'utilisateur
    /// </summary>
    public void Logout()
    {
        _currentUser = null;
        NotifyAuthStateChanged();
    }

    /// <summary>
    /// Notifier tous les composants abonnés que l'état a changé
    /// </summary>
    private void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }
}