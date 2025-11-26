using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.ViewModels;

public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty]
    private int _currentCount = 0;
    
    [ObservableProperty]
    private CounterUtilisateurViewDto? _utilisateur;
    
    [RelayCommand]
    private void IncrementCount()
    {
        
        CurrentCount++;
    }
}
