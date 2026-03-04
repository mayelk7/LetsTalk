using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.Context;

public sealed class UserContext
{
    public UserDto? CurrentUser { get; set; }
    
    public bool IsConnected => CurrentUser != null;
}
