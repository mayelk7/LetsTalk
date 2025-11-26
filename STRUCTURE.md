# Structure et rôles des dossiers

Ce document décrit, en français, le rôle des dossiers fréquemment utilisés dans ce projet : `ViewModels`, `Models`, `Views`, `ModelsDto` (ou `ModelsDTO`), `Migrations`, `Context` et `Components`. Il vise à aider les contributeurs à savoir **où** placer le code et **quelles responsabilités** attribuer à chaque couche.

## Résumé rapide

- **ViewModels** : logique de présentation et commandes (pattern MVVM côté client).
- **Views** : UI (Razor pages / composants) et liaisons aux ViewModels.
- **Models** : entités persistées côté serveur (domain model / EF Core).
- **ModelsDto** : objets de transfert (DTO) partagés entre client et serveur.
- **Context** : configuration du `DbContext` (EF Core) et mappings.
- **Migrations** : fichiers générés par EF Core pour l'historique du schéma.
- **Components** : composants réutilisables côté UI (Razor Components).

---

## 1) ViewModels

### Vue d’ensemble

- **Dossier principal** : `LetsTalk.Client/ViewModels/`
- **Rôle** : couche de **présentation** qui fait le lien entre :
  - les **données** (services, API, DTOs),
  - et la **Vue** (Razor) via des propriétés observables et des commandes.
- Utilise `CommunityToolkit.Mvvm` (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`).

### Responsabilités

- Exposer l’état affiché par l’UI : propriétés simples (`string`, `int`, DTOs, etc.).
- Exposer des **commandes** (méthodes marquées `[RelayCommand]`) déclenchées par l’UI.
- Coordonner la logique métier de « haut niveau » :
  - appels à des services (ex. API REST, services locaux),
  - validation simple côté client,
  - navigation, messages, etc.

### Ne doit pas

- Accéder directement à la base de données ou au `DbContext`.
- Contenir de logique de persistance (INSERT/UPDATE/DELETE SQL).
- Connaître les détails d’EF Core (migrations, relations, etc.).

### Exemple concret (ViewModel simple)

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LetsTalk.Client.ViewModels;

public partial class MonViewModel : ObservableObject
{
    [ObservableProperty]
    private string _monTexte = "Valeur initiale";

    [ObservableProperty]
    private int _compteur;

    [RelayCommand]
    private void Incremente()
    {
        Compteur++;
        MonTexte = $"Compteur : {Compteur}";
    }
}
```

### Workflow type

1. L’utilisateur clique sur un bouton dans la **View**.
2. La View appelle une **commande** du ViewModel (`IncrementeCommand`).
3. Le ViewModel met à jour ses **propriétés observables** (`Compteur`, `MonTexte`).
4. L’UI se met à jour automatiquement grâce au binding / au `PropertyChanged`.

---

## 2) Views

### Vue d’ensemble

- **Dossier principal (client)** : `LetsTalk.Client/Views/`
- **Autres emplacements** : composants partagés dans `LetsTalk/Components/`.
- **Rôle** : définir le **markup** (HTML + Razor) et le **style** qui s’affichent dans le navigateur.

### Responsabilités

- Définir les routes (`@page "/ma-page"`).
- Injecter le ViewModel (`@inject MonViewModel Vm`).
- Faire le **binding** des propriétés (`@Vm.Compteur`) et des événements (`@onclick`).
- Appliquer du style, du layout, de l’affichage conditionnel.

### Ne doit pas

- Contenir de logique métier lourde ou de calcul complexe.
- Appeler directement la base de données.
- Dupliquer la logique déjà présente dans le ViewModel.

### Exemple Razor lié à `MonViewModel`

```razor
@page "/ma-page"
@inject LetsTalk.Client.ViewModels.MonViewModel Vm

<h3>@Vm.MonTexte</h3>
<p>Valeur : @Vm.Compteur</p>

<button class="btn btn-primary" @onclick="Vm.Incremente">Incrémenter</button>

@code {
    protected override void OnInitialized()
    {
        // Si besoin d'écouter les changements manuellement
        Vm.PropertyChanged += (_, __) => StateHasChanged();
        base.OnInitialized();
    }
}
```

### Workflow type

- La View se contente de **réagir** aux données du ViewModel et de **relayer** les événements utilisateur.

---

## 3) Models (Entités domaine / EF Core)

### Vue d’ensemble

- **Dossier principal** : `LetsTalk/Models/`
- **Rôle** : représenter les **entités métier** qui seront **persistées** en base via EF Core.

### Responsabilités

- Définir les propriétés qui correspondent (souvent) aux colonnes d’une table.
- Porter certaines règles métier simples (ex. champs requis, longueurs maximales).
- Définir les relations entre entités (navigation properties, clés étrangères).

### Ne doit pas

- Connaître l’UI (pas de dépendance vers des ViewModels ou Views).
- Connaître des DTOs ou des formats de transport (JSON, etc.).

### Exemple d’entité `Utilisateur`

```csharp
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Models;

public class Utilisateur
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    // Propriétés supplémentaires
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
```

### Workflow type

- Le `DbContext` expose un `DbSet<Utilisateur>`.
- EF Core génère une migration basée sur ce modèle.
- L’application lit / écrit des instances de `Utilisateur` via le `DbContext`.

---

## 4) ModelsDto (DTO)

### Vue d’ensemble

- **Dossier principal** : `LetsTalk.Shared/ModelsDTO/`
- **Rôle** : modèles de **transport de données** entre :
  - le **serveur** (projet `LetsTalk`),
  - le **client** (projet `LetsTalk.Client`).

### Responsabilités

- Simplifier / adapter les entités pour l’API (ne contient que ce qui est nécessaire).
- Servir de contrat stable entre client et serveur (versionning d’API, etc.).

### Ne doit pas

- Contenir de logique métier complexe.
- Référencer des types d’EF Core (migration, DbContext, etc.).

### Exemple DTO + mapping manuel

```csharp
namespace LetsTalk.Shared.ModelsDTO;

public record UtilisateurDto(int Id, string Nom, string Email);

// Mapping manuel (service ou extension)
public static class UtilisateurMapping
{
    public static UtilisateurDto ToDto(this LetsTalk.Models.Utilisateur u)
        => new UtilisateurDto(u.Id, u.Nom, u.Email);

    public static LetsTalk.Models.Utilisateur ToEntity(this UtilisateurDto dto)
        => new LetsTalk.Models.Utilisateur { Id = dto.Id, Nom = dto.Nom, Email = dto.Email };
}
```

### Workflow type

1. Le backend récupère une entité `Utilisateur` depuis la base.
2. Il la convertit en `UtilisateurDto` avant de répondre à l’API.
3. Le client reçoit le `UtilisateurDto` et l’utilise dans ses ViewModels / Views.

---

## 5) Context (EF Core / AppDbContext)

### Vue d’ensemble

- **Dossier principal** : `LetsTalk/Context/`
- **Fichier clé** : `AppDbContext.cs`.
- **Rôle** : point central d’accès à la base via EF Core.

### Responsabilités

- Déclarer les `DbSet<T>` pour chaque entité persistée.
- Configurer le schéma (conventions, index, relations) dans `OnModelCreating`.
- Être enregistré dans le **container DI** dans `Program.cs` (via `AddDbContext<AppDbContext>`).

### Exemple `AppDbContext` minimal

```csharp
using Microsoft.EntityFrameworkCore;
using LetsTalk.Models;

namespace LetsTalk.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Utilisateur>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Nom).HasMaxLength(100).IsRequired();
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
        });
    }
}
```

### Lien avec `Program.cs`

Dans `LetsTalk/Program.cs` :

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
```

---

## 6) Migrations

### Vue d’ensemble

- **Dossier principal** : `LetsTalk/Migrations/`
- Fichiers générés automatiquement par EF Core (ne pas les éditer à la main sauf cas très particulier).

### Responsabilités

- Représenter l’**historique** des changements du schéma de base.
- Fournir des méthodes `Up` (appliquer) et `Down` (annuler) pour chaque migration.

### Exemple (extrait simplifié)

```csharp
public partial class CreateTableUtilisateur : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Utilisateurs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Utilisateurs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Utilisateurs_Email",
            table: "Utilisateurs",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Utilisateurs");
    }
}
```

### Workflow type

- Modifier un Model ou le DbContext →
- Générer une migration (`dotnet ef migrations add Nom`) →
- Appliquer la migration (`dotnet ef database update`).

---

## 7) Components (composants Razor réutilisables)

### Vue d’ensemble

- **Dossiers possibles** :
  - `LetsTalk/Components/` (côté serveur),
  - ou dans le client si composants spécifiques au front.
- **Rôle** : factoriser du code UI réutilisable (boutons, formulaires, layouts, sections d’écran, etc.).

### Responsabilités

- Encapsuler du markup + comportement UI.
- Exposer des paramètres (`[Parameter]`) et des événements (`EventCallback`).

### Exemple `MyButton.razor`

```razor
<button class="btn @Class" @onclick="OnClick">
    @ChildContent
</button>

@code {
    [Parameter]
    public string Class { get; set; } = "btn-primary";

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
```

Utilisation dans une page :

```razor
<MyButton Class="btn-success" OnClick="() => Console.WriteLine("Clicked")">
    Cliquer
</MyButton>
```

---

## 8) Vue globale du workflow

Pour résumer le cheminement typique des données :

1. **Utilisateur** interagit avec une **View** (clic sur un bouton, saisie dans un formulaire).
2. La View appelle une **commande** du **ViewModel**.
3. Le ViewModel appelle un **service** / une API, qui utilise des **DTOs** (`ModelsDto`).
4. Le serveur convertit les DTOs en **Models** (entités) et utilise le **DbContext** (`Context`) pour lire/écrire en base.
5. Si la structure change, l’évolution passe par une **Migration**.
6. Le résultat remonte sous forme de DTO, puis mis en forme dans le ViewModel et affiché par la View / les Components.

---

## Annexe — Contrats rapides et cas limites

- **ViewModel** :
  - Input = services / événements UI,
  - Output = propriétés observables + commandes.
  - Cas limites : états null, erreurs réseau, initialisation asynchrone.

- **View** :
  - Input = ViewModel / paramètres,
  - Output = rendu UI + interactions utilisateur.
  - Cas limites : chargement lent, responsive, accessibilité.

- **Model / DbContext** :
  - Input = migrations / requêtes ORM,
  - Output = entités persistées.
  - Cas limites : conflits de schéma, migrations manquantes, timeouts DB.

- **DTO** :
  - Input = entité,
  - Output = payload JSON sérialisé.
  - Cas limites : changement de version d’API, champs optionnels.

---

Si vous voulez, je peux :
- Ajouter un diagramme (mermaid) qui illustre ce workflow,
- Créer un `CONTRIBUTING.md` qui impose cette structure (où créer quoi / checklist),
- Ajouter des exemples de tests unitaires pour un ViewModel.
