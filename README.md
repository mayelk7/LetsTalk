# LetsTalk
Un petit guide pour comprendre la structure du projet LetsTalk, comment le configurer et le lancer localement.
## Aperçu du dépôt
Le dépôt contient une solution multi-projet pour une application Blazor/MVVM :
- `LetsTalk/` — Projet serveur (ASP.NET Core) qui expose des composants interactifs, configure EF Core et sert l'application.
  - `Program.cs` : configuration et démarrage de l'application serveur.
  - `Context/` : `AppDbContext.cs` (configuration EF Core).
  - `Migrations/` : migrations EF Core.
  - `Models/` : entités côté serveur.
  - `Components/` et `wwwroot/` : ressources et composants Razor.
- `LetsTalk.Client/` — Client Blazor WebAssembly (projet front-end). Contient les `Views/`, `ViewModels/` et les ressources front.
  - `Program.cs` : point d'entrée WebAssembly.
  - `ViewModels/` : ViewModels (ex. `CounterViewModel.cs`).
  - `Views/` : pages et layout.
- `LetsTalk.Shared/` — Types partagés entre le client et le serveur (DTOs, modèles légers).
La solution racine contient `LetsTalk.sln`.
## Prérequis
- .NET 9 SDK (ou version compatible utilisée par le projet)
- MySQL / MariaDB si vous utilisez la connexion configurée par défaut
- (Optionnel) dotnet-ef pour gérer les migrations :
## Configuration

Les fichiers de configuration sont situés dans :

- `LetsTalk/appsettings.json` et `LetsTalk/appsettings.Development.json`
- `LetsTalk.Client/appsettings.json` et `LetsTalk.Client/appsettings.Development.json`

Assurez-vous de renseigner la chaîne de connexion nommée `DefaultConnection` dans `LetsTalk/appsettings.json` avant de lancer les migrations ou de démarrer l'application.
## Construire et lancer l'application (locale)

Depuis la racine du dépôt :

1) Restaurer les packages et compiler :

```bash
dotnet restore
dotnet build
```

2a) Lancer le projet serveur (recommandé) — le serveur expose les composants interactifs et peut servir le client :
dotnet run --project LetsTalk/LetsTalk.csproj
2b) (Optionnel) Lancer uniquement le client WebAssembly :
dotnet run --project LetsTalk/LetsTalk.Client/LetsTalk.Client.csproj
Accédez ensuite à l'URL indiquée dans la sortie (généralement https://localhost:nnnn).

## EF Core — Migrations
Exemples de commandes (depuis la racine du dépôt). Ici on cible explicitement le projet serveur :

- Créer une nouvelle migration :

```bash
dotnet ef migrations add NomDeLaMigration --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```
- Appliquer les migrations à la base de données :

```bash
dotnet ef database update --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```
- Supprimer la dernière migration (non appliquée) :

```bash
dotnet ef migrations remove --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```
- Lister les migrations :

```bash
dotnet ef migrations list --project LetsTalk/LetsTalk.csproj --startup-project LetsTalk/LetsTalk.csproj
```
Notes :
- Si `dotnet ef` signale des erreurs, vérifiez que le SDK et le package provider (ex. Pomelo.EntityFrameworkCore.MySql) sont correctement restaurés.
- Vous pouvez exécuter les commandes EF depuis le dossier `LetsTalk/` en enlevant les paramètres `--project`/`--startup-project`.

## Où modifier / ajouter du code

- ViewModels : `LetsTalk.Client/ViewModels/`
- Views : `LetsTalk.Client/Views/`
- Entités / modèles serveur : `LetsTalk/Models/`
- Contexte EF Core : `LetsTalk/Context/AppDbContext.cs`

## Contribution

- Créez une branche feature, testez localement et ouvrez une MR/PR vers la branche principale.
- Pour des changements de schéma : ajoutez une migration et testez l'application après `dotnet ef database update`.

---

Si vous voulez, je peux :
- Ajouter des instructions de debug plus détaillées (ex : variables d'environnement, ports),
- Générer un script de lancement pour Windows (cmd) ou PowerShell,
- Ajouter un fichier CONTRIBUTING.md ou des badges CI.

Indiquez ce que vous souhaitez que j'ajoute ensuite.
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

