using LetsTalk.Shared.Enum;

namespace LetsTalk.Shared.ModelsDto;

public record FichierMessageDto(int? Id, string? Nom, string Url, MessageType? Type);