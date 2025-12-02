using CommunityToolkit.Mvvm.ComponentModel;

namespace LetsTalk.Client.ViewModels;

public partial class ServerViewModel : ObservableObject
{
    private int ServerId { get; set; }

    [ObservableProperty]
    private Server? _server;
    
    [ObservableProperty]
    private Channel? _selectedChannel;

    public void Initialize(int serverId)
    {
        // TODO: Load server data based on serverId
        ServerId = serverId;
        Server = new Server(ServerId, "Server " + ServerId, [
            new Channel(1, "General", ChannelType.Text),
            new Channel(2, "Random", ChannelType.Text),
            new Channel(3, "Voice Channel", ChannelType.Voice)
        ], [
            new User(1, "Alice"),
            new User(2, "Bob"),
            new User(3, "Charlie")
        ]);
        
        SelectedChannel = Server.Channels[0];
    }
}

// TODO: Temporary data structures until real data models are implemented
public record Server (int Id, string Name, List<Channel> Channels, List<User> Users);
public record User (int Id, string Username);
public enum ChannelType { Text = 0, Voice = 1 }
public record Channel (int Id, string Name, ChannelType Type);