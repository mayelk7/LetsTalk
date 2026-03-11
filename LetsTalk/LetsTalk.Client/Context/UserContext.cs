using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.Context;

public class UserContext
{
    public UserDto? CurrentUser { get; set; }
    
    public bool IsConnected => CurrentUser != null;
}
