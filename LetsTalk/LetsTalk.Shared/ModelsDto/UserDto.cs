namespace LetsTalk.Shared.ModelsDto;

public record UserDto(int? Id, string Username, string Email, string Phone, string? ProfilPicture, DateTime CreatedAt);