namespace LetsTalk.Shared.ModelsDto;

public record FullServerDto(int? Id, string Name, List<ChannelDto> Channels, List<UserDto> Users);