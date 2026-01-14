using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.ViewModels;

public partial class ServerViewModel : ObservableObject
{
    private int ServerId { get; set; }

    [ObservableProperty]
    private FullServerDto? _server;
    
    [ObservableProperty]
    private ChannelDto? _selectedChannel;

    [ObservableProperty]
    private string? _currentMessage;
    
    public void OnMessageInput()
    {
        MessageDto message = new(
            0,
            new UserDto(
                999,
                "CurrentUser" // TODO: Replace with actual current user
            ),
            CurrentMessage ?? string.Empty,
            DateTime.Now
        );
        
        SelectedChannel?.Messages.Add(message);
        CurrentMessage = string.Empty;
    }
    
    public async Task InitAsync(int serverId)
    {
        Server = await ApiManagerService.MakeGetRequest<FullServerDto>("/api/server/" + serverId);
        
        ServerId = serverId;
        
        
        SelectedChannel = Server?.Channels.Count > 0 
            ? Server?.Channels[0]
            : null;
    }
}
