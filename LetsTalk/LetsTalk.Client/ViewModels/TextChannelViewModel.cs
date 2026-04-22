using CommunityToolkit.Mvvm.ComponentModel;
using LetsTalk.Client.Services;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.ViewModels;

public partial class TextChannelViewModel(AuthStateService authState) : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    private PeriodicTimer _refreshTimer = new(TimeSpan.FromSeconds(1));
    
    [ObservableProperty]
    private AuthStateService _authState = authState;
    
    [ObservableProperty]
    private List<MessageCanalDto>? _messages = [];

    public void InitAsync()
    {
        _ = this.GetMessagesAsync();

        _ = this.RefreshLoopAsync();
    }

    public void RefreshMessages()
    {
        _ = this.GetMessagesAsync();
    }

    public void StopRefreshMessages()
    {
        this._refreshTimer.Dispose();
    }

    private async Task RefreshLoopAsync()
    {
        while (await _refreshTimer.WaitForNextTickAsync())
        {
            await GetMessagesAsync();
        }
    }

    private async Task GetMessagesAsync()
    {
        var response = await ApiManagerService.MakeGetRequest<List<MessageCanalDto>>("/api/messagecanaux/canal/" + Id);
        
        if (response is { Success: false })
        {
            Console.WriteLine("Error fetching server data: " + response.Message);
            return;
        }
        
        Messages = response?.Data;
    }
}
