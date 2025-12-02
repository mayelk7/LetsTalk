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

        User user1 = new (1, "User 1");
        User user2 = new (2, "User 2");
        
        ServerId = serverId;
        
        Server s1 = new Server(ServerId, "Server " + ServerId, [
            new Channel(1, "General", [
                new Message(1, user1, "Tu es libre vendredi soir pour dîner ?", DateTime.Now.AddMinutes(-10)),
                new Message(2, user2, "Oui, ça devrait aller. Tu penses à quel resto ?", DateTime.Now.AddMinutes(-5)),
                new Message(3, user1, "J’hésite entre un italien ou un japonais.", DateTime.Now),
                new Message(4, user2, "Italien, ça me tente bien ! On réserve ?", DateTime.Now.AddMinutes(2)),
                new Message(5, user1, "Parfait, je m’en occupe.", DateTime.Now.AddMinutes(3))
            ], ChannelType.Text),
            
            new Channel(2, "Random", [
                new Message(1, user1, "As-tu avancé sur le projet de sciences ?", DateTime.Now.AddMinutes(-10)),
                new Message(2, user2, "Un peu, j’ai fait l’intro, mais le reste est encore vide.", DateTime.Now.AddMinutes(-5)),
                new Message(3, user1, "Super, j’écris la partie sur les expériences alors.", DateTime.Now),
                new Message(4, user2, "Ça marche. On relit tout ensemble demain ?", DateTime.Now.AddMinutes(2)),
                new Message(5, user1, "Oui, après les cours !", DateTime.Now.AddMinutes(3))
            ],ChannelType.Text),
            
            new Channel(3, "Voice Channel", [
                new Message(1, user1, "On regarde quoi ce soir ?", DateTime.Now.AddMinutes(-10)),
                new Message(2, user2, "Un film d’action ?", DateTime.Now.AddMinutes(-5)),
                new Message(3, user1, "Pourquoi pas, mais pas trop long.", DateTime.Now),
                new Message(4, user2, "Alors Baby Driver ? C’est dynamique et pas trop long.", DateTime.Now.AddMinutes(2)),
                new Message(5, user1, "Parfait, je lance !", DateTime.Now.AddMinutes(3))
            ],ChannelType.Voice)
        ], [
            new User(1, "Alice"),
            new User(2, "Bob"),
            new User(3, "Charlie")
        ]);
        
        Server s2 = new Server(ServerId + 1, "Server " + (ServerId + 1), [
            new Channel(1, "General", [
                new Message(1, user1, "Salut Bob, tu bosses sur quoi aujourd’hui ?", DateTime.Now.AddMinutes(-12)),
                new Message(2, user2, "Je finis le rapport pour le boulot, pas simple…", DateTime.Now.AddMinutes(-9)),
                new Message(3, user1, "Bon courage ! Si tu veux, je peux relire.", DateTime.Now.AddMinutes(-7)),
                new Message(4, user2, "Volontiers, ce serait top !", DateTime.Now.AddMinutes(-4)),
                new Message(5, user1, "Envoie-moi ça quand tu veux.", DateTime.Now.AddMinutes(-2))
            ], ChannelType.Text),
    
            new Channel(2, "Random", [
                new Message(1, user2, "J’ai découvert un nouveau café super sympa.", DateTime.Now.AddMinutes(-15)),
                new Message(2, user1, "Ah oui ? Où ça ?", DateTime.Now.AddMinutes(-11)),
                new Message(3, user2, "Près du parc central, super ambiance.", DateTime.Now.AddMinutes(-8)),
                new Message(4, user1, "Il faut que j’essaie alors !", DateTime.Now.AddMinutes(-5)),
                new Message(5, user2, "Tu vas adorer, c’est sûr.", DateTime.Now.AddMinutes(-3))
            ], ChannelType.Text),
    
            new Channel(3, "Gaming", [
                new Message(1, user2, "Quelqu’un chaud pour jouer ce soir ?", DateTime.Now.AddMinutes(-20)),
                new Message(2, user1, "Moi, je suis disponible après 21h.", DateTime.Now.AddMinutes(-16)),
                new Message(3, user2, "Parfait, on se fait une partie de Rocket League ?", DateTime.Now.AddMinutes(-12)),
                new Message(4, user1, "Carrément !", DateTime.Now.AddMinutes(-9)),
                new Message(5, user2, "Allez, à tout à l’heure !", DateTime.Now.AddMinutes(-6))
            ], ChannelType.Voice)
        ], [
            new User(1, "Alice"),
            new User(2, "Bob"),
            new User(3, "Charlie")
        ]);
        
        Server = ServerId % 2 == 0 ? s1 : s2;
        
        SelectedChannel = Server.Channels[0];
    }
}

// TODO: Temporary data structures until real data models are implemented
public record Server (int Id, string Name, List<Channel> Channels, List<User> Users);
public record User (int Id, string Username);
public enum ChannelType { Text = 0, Voice = 1 }
public record Channel (int Id, string Name, List<Message> Messages, ChannelType Type);
public record Message (int Id, User User, string Content, DateTime Timestamp);