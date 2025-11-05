using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LetsTalk.Client.ViewModels;

public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty]
    private int _currentCount = 0;
    
    [RelayCommand]
    private void IncrementCount()
    {
        CurrentCount++;
    }
}