

namespace LetsTalk.Shared.ModelsDto;

public record ParticipantLiveKit
(        // Infos LiveKit
        int Identity, // regarder que jsp la quelle est le quelle
        string Name, //surement pas necessaire
        string State, // oui important
        bool CanPublish, // important pour savoir si il peu parler
        bool CanSubscribe, // important pour savoir si il peu ecoutet 
        //public bool IsAudioMuted { get; set; } on verra plus tard
        //public bool IsSpeaking { get; set; } on verra plus tard

        // Infos depuis ta BDD
        string AvatarUrl,
        string Username
        
);

