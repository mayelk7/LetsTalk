namespace LetsTalk.Shared.ModelsDto;

public record MessageCanalDto(
    int MessageId,
    string Contenu,
    DateTime DateEnvoi,

    int? UtilisateurId,
    string Username,

    int CanalId,
    string NomCanal,
    string? File
);
