namespace LetsTalk.Shared.ModelsDto;

public record FullUser(int Id, string Username, string Email, string Phone, string? ProfilPicture, DateTime CreatedAt);