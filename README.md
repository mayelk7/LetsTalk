# LetsTalk - Guide MVVM

## Création d'une View et ViewModel

### 1. Créer le ViewModel

**Emplacement :** `LetsTalk.Client/ViewModels/`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LetsTalk.Client.ViewModels;

public partial class MonViewModel : ObservableObject
{
    // Propriété observable
    [ObservableProperty]
    private string _monTexte = "Valeur initiale";
    
    // Commande
    [RelayCommand]
    private void MonAction()
    {
        MonTexte = "Nouvelle valeur";
    }
}
```

### 2. Enregistrer le ViewModel

**Fichier :** `LetsTalk/Program.cs`

```csharp
builder.Services.AddScoped<MonViewModel>();
```

### 3. Créer la View

**Emplacement :** `LetsTalk.Client/Views/`

```razor
@page "/ma-page"
@inject MonViewModel Vm

<PageTitle>Ma Page</PageTitle>

<MudText Typo="Typo.h3">@Vm.MonTexte</MudText>

<MudButton Color="Color.Primary" 
           Variant="Variant.Filled" 
           @onclick="Vm.MonActionCommand.Execute">
    Cliquer ici
</MudButton>

@code {
    protected override void OnInitialized()
    {
        Vm.PropertyChanged += (_, __) => StateHasChanged();
        base.OnInitialized();
    }
}
```

## Points clés

- **`[ObservableProperty]`** : Génère automatiquement la propriété publique et `INotifyPropertyChanged`
- **`[RelayCommand]`** : Génère automatiquement une commande avec le suffixe `Command`
- **`@inject`** : Injecte le ViewModel dans la View
- **`Vm.PropertyChanged += ...`** : Synchronise l'UI avec les changements du ViewModel
- **CommunityToolkit.Mvvm** : Simplifie le pattern MVVM avec des source generators

## Structure du projet

```
LetsTalk.Client/
├── ViewModels/          # Logique métier
│   └── MonViewModel.cs
└── Views/               # Interface utilisateur
    └── MaPage.razor
```

