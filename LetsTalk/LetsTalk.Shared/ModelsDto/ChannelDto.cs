using LetsTalk.Shared.Enum;

namespace LetsTalk.Shared.ModelsDto;

public record ChannelDto(int? Id, string Name, List<MessageDto> Messages, ChannelType ChannelType);