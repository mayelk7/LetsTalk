namespace LetsTalk.Shared.ModelsDto;

public record MessageDto(int? Id, UserDto Sender, string Content, DateTime Timestamp, int CanalId, bool Epingle, FichierMessageDto? Fichier);