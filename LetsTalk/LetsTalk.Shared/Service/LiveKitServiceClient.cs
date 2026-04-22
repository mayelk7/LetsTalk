using Livekit.Server.Sdk;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetsTalk.Shared;
using Livekit.Server.Sdk.Dotnet;

namespace LetsTalk.Shared.Service
{
    public class LiveKitServiceClient
    {
        public string GenerateAccessToken(string roomName, string identity)
        {
            // 1. Définir les autorisations
            var grant = new VideoGrants
            {
                Room = roomName,           // Nom du salon (autorise implicitement le join)
                CanPublish = true,         // Peut publier de l'audio/vidéo
                CanSubscribe = true,       // Peut s'abonner aux autres pistes
                CanUpdateOwnMetadata = true,
                RoomJoin = true
            };

            // 2. Créer le Jeton d'Accès
            var token = new AccessToken("devkey", "6881b60016700ea19aee9137b802a557a93e8d074e1bdb2e1d4f21948a0d06ca")
                .WithIdentity(identity)
                .WithTtl(TimeSpan.FromHours(2)) // Valide pendant 2 heures
                .WithGrants(grant);

            // 3. Retourner le JWT
            return token.ToJwt();
        }
    }

}
