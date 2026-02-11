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
                1,
                "CurrentUser", // TODO: Replace with actual current user
                "john.doe@exemple.com",
                "1234567890",
                null,
                DateTime.Now
            ),
            CurrentMessage ?? string.Empty,
            DateTime.Now,
            SelectedChannel?.Id ?? 0,
            false
        );
        
        SelectedChannel?.Messages.Add(message);
        
        CurrentMessage = string.Empty;
        
        ApiManagerService.MakePostRequest<MessageCanalDto?,MessageDto>("/api/messagecanaux", message)
            .ContinueWith(
            response =>
            {
                if (response.Result is {Success: false})
                {
                    Console.WriteLine("Error sending message: " + response.Result.Message);
                    return Task.CompletedTask;
                }
                
                Console.WriteLine("Message sent: " + response.Result?.Message);
                
                return Task.CompletedTask;
            });
    }
    
    public async Task InitAsync(int serverId)
    {
        await this.LoadServerDataAsync(serverId);
    }

    private async Task LoadServerDataAsync(int serverId)
    {
        var response = await ApiManagerService.MakeGetRequest<FullServerDto>("/api/server/" + serverId);

        if (response is {Success: false})
        {
            Console.WriteLine("Error fetching server data: " + response.Message);
            return;
        }
        
        Server = response?.Data;
        
        ServerId = serverId;
        
        SelectedChannel = Server?.Channels.Count > 0 
            ? Server?.Channels[0]
            : null;
    }
}
