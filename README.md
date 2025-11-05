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

## EFCore - Migrations

### Installation de l'outil CLI

Une classe Expemple est dans le Dossier Models dans LetsTalk.Client, pensez bien à mettre les attribus de Primary Key et Index.
Pour qu'il se rajoute dans la base il faut également ajouter dans AppDbContext.cs votre classe comme sr l'exemple.

```bash
dotnet tool install --global dotnet-ef
```

### Créer une migration

Après avoir modifié vos modèles ou `AppDbContext`, créez une migration :

```bash
dotnet ef migrations add InitialMigration --project LetsTalk --startup-project LetsTalk
```

> `InitialMigration` est le nom de la migration (à adapter selon vos changements)

### Appliquer la migration à la base de données

```bash
dotnet ef database update --project LetsTalk --startup-project LetsTalk
```

### Commandes utiles

- **Supprimer la dernière migration (non appliquée) :**
  ```bash
  dotnet ef migrations remove --project LetsTalk --startup-project LetsTalk
  ```

- **Lister les migrations :**
  ```bash
  dotnet ef migrations list --project LetsTalk --startup-project LetsTalk
  ```

- **Revenir à une migration spécifique :**
  ```bash
  dotnet ef database update NomDeLaMigration --project LetsTalk --startup-project LetsTalk
  ```

