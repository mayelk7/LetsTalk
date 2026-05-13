using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Context;
using LetsTalk.Client.Services;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Components.Forms;

namespace LetsTalk.Client.ViewModels;

public partial class ServerViewModel(UserContext userContext, AuthStateService authState) : ObservableObject
{
    [ObservableProperty]
    private UserContext _userContext = userContext;

    private readonly AuthStateService _authState = authState;

    [ObservableProperty] 
    private int _serverId;

    [ObservableProperty]
    private FullServerDto? _server;
    
    [ObservableProperty]
    private ChannelDto? _selectedChannel;

    [ObservableProperty]
    private string? _currentMessage;

    [ObservableProperty] 
    private IBrowserFile? _file;
    
    public async Task OnMessageInput()
    {
        if (CurrentMessage == null || SelectedChannel == null)
        {
            return;
        }
        
        CurrentMessage = CurrentMessage.Trim();
        
        if (CurrentMessage.Length == 0)
        {
            return;
        }

        FichierMessageDto? fichier = null;
        
        Console.WriteLine("Current message: " + CurrentMessage);
        Console.WriteLine("Current file: " + (File != null ? File?.Name : "No file"));

        if (File != null)
        {
            var buffer = new byte[File?.Size ?? 0];
            _ = File?.OpenReadStream().ReadAsync(buffer);
            
            Console.WriteLine(Convert.ToBase64String(buffer));

            fichier = new FichierMessageDto(
                0,
                File?.Name ?? "Inconnu",
                Convert.ToBase64String(buffer),
                MessageType.Canal
            );
        }
        
        var currentUser = _authState.CurrentUser;

        if (currentUser == null) return;

        MessageDto message = new(
            0,
            new UserDto(
                currentUser.UtilisateurId,
                currentUser.Username,
                currentUser.Email,
                currentUser.Phone,
                currentUser.ProfilPicture,
                currentUser.CreatedAt
            ),
            CurrentMessage ?? string.Empty,
            DateTime.Now,
            SelectedChannel.Id ?? 0,
            false,
            fichier
        );
        
        SelectedChannel.Messages.Add(message);
        
        CurrentMessage = string.Empty;
        File = null;
        
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

        if (File != null)
        {
            Console.WriteLine("File selected: " + File.Name);
        }
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
