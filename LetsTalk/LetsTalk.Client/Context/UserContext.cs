using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.Context;

public partial class UserContext : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsConnected))]
    private UserDto? _currentUser;

    public bool IsConnected => CurrentUser != null;
}