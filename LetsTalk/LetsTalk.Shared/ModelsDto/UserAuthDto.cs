namespace LetsTalk.Shared.ModelsDto
{
    public record UserAuthDto
    (
        int UtilisateurId,
        string Username,
        string Email,
        string Phone,
        string? ProfilPicture,
        bool Actif,
        DateTime CreatedAt
    );
}
