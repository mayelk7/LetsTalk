# Guide de contribution à LetsTalk

Merci de contribuer à ce projet ! Ce document décrit comment organiser votre code (ViewModels, Models, DTOs, etc.) et le workflow recommandé pour proposer des changements.

## 1. Pré-requis

- .NET SDK (version utilisée par le projet, actuellement .NET 9).
- MySQL / MariaDB si vous travaillez sur la base de données.
- Outil EF Core CLI (facultatif mais recommandé) :

```bash
dotnet tool install --global dotnet-ef
```

## 2. Organisation du code (où mettre quoi)

Reportez-vous au fichier `STRUCTURE.md` pour une description détaillée. Rappel rapide :

- `LetsTalk.Client/ViewModels/` : ViewModels (logique de présentation, commandes).
- `LetsTalk.Client/Views/` : Views (pages/composants Razor côté client).
- `LetsTalk/Models/` : entités métier persistées (EF Core).
- `LetsTalk.Shared/ModelsDTO/` : DTOs partagés entre client et serveur.
- `LetsTalk/Context/` : `AppDbContext` et configuration EF Core.
- `LetsTalk/Migrations/` : migrations générées par EF Core.
- `LetsTalk/Components/` : composants Razor réutilisables.

**Règle d’or** :
- View / ViewModel ne parlent pas directement au `DbContext`.
- Le serveur ne renvoie jamais les entités EF directement au client : toujours passer par des DTOs.

## 3. Workflow de développement

### 3.1. Créer une nouvelle fonctionnalité UI

1. **Créer/Mettre à jour un ViewModel** dans `LetsTalk.Client/ViewModels/` :
   - Ajouter des propriétés `[ObservableProperty]`.
   - Ajouter des commandes `[RelayCommand]`.
2. **Créer/Mettre à jour une View** dans `LetsTalk.Client/Views/` :
   - Définir la route `@page` si c’est une nouvelle page.
   - Injecter le ViewModel avec `@inject`.
3. Si besoin d’appeler le backend :
   - Créer/Mettre à jour un DTO dans `LetsTalk.Shared/ModelsDTO/`.
   - Appeler l’API depuis le ViewModel via un service/HttpClient.

### 3.2. Modifier le modèle de données (base de données)

1. **Modifier ou ajouter une entité** dans `LetsTalk/Models/`.
2. **Mettre à jour `AppDbContext`** si nécessaire (`DbSet`, relation, configuration).
3. **Générer une migration EF Core** :

```bash
dotnet ef migrations add NomDeLaMigration --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```

4. **Appliquer la migration** :

```bash
dotnet ef database update --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```

5. Si ces entités sont exposées au client, créer/adapter les DTOs (`LetsTalk.Shared/ModelsDTO/`) et leur mapping.

## 4. Bonnes pratiques de code

- **Nommage** :
  - ViewModel : suffixe `ViewModel` (ex. `CounterViewModel`).
  - DTO : suffixe `Dto` ou `...ViewDto` (ex. `UtilisateurDto`).
  - Migrations : nom explicite (ex. `AddUtilisateurTable`, `AddIndexEmailOnUtilisateur`).

- **Séparation des responsabilités** :
  - Ne mélangez pas la logique UI (Razor) et la logique métier (ViewModel).
  - Ne mettez pas de logique réseau directement dans les Views.

- **Tests** (optionnel mais recommandé) :
  - Les ViewModels doivent être testables sans dépendre de l’UI.
  - Préférez injecter des interfaces (services) plutôt que d’utiliser des singletons statiques.

## 5. Processus de Pull Request (PR)

1. **Créer une branche** à partir de `main` (ou de la branche cible) :

```bash
git checkout -b feature/ma-fonctionnalite
```

2. **Implémenter les changements** en respectant la structure :
   - ViewModels, Views, Models, DTOs, etc. dans les bons dossiers.
3. **Mettre à jour la base** si nécessaire :
   - Générer les migrations et les appliquer localement.
4. **Lancer les builds/tests** :

```bash
dotnet build
# et idéalement
# dotnet test
```

5. **Ouvrir une Pull Request** en décrivant :
   - Le but de la fonctionnalité.
   - Les changements principaux (fichiers/dossiers concernés).
   - Les impacts sur la base de données (s’il y en a).

## 6. Style et conventions

- Code C# : respecter les conventions standard (.NET) et celles existantes dans le projet.
- Utiliser les Source Generators `CommunityToolkit.Mvvm` pour les ViewModels plutôt que d’implémenter manuellement `INotifyPropertyChanged`.
- Garder les fichiers de configuration (`appsettings.json`) cohérents et **ne jamais** commiter de secrets.

## 7. Questions / améliorations

Si vous hésitez sur l’emplacement d’une classe ou sur la façon de structurer une nouvelle fonctionnalité :

- Consultez d’abord `STRUCTURE.md`.
- Inspirez-vous des exemples existants (`CounterViewModel`, `Counter.razor`, `CounterUtilisateurViewDto`, etc.).
- En cas de doute, ouvrez une issue ou discutez dans la PR.

Merci pour vos contributions !

