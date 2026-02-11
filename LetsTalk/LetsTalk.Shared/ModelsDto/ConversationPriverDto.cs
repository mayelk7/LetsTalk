namespace LetsTalk.Shared.ModelsDto;

public record ConversationPriverDto(
    int ConversationPriverId,
    string? ConversationNom,    // NULL autorisé en base
    DateTime CreatedAt
);