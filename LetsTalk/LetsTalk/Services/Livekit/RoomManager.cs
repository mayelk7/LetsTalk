using LetsTalk.Context;
using LetsTalk.Shared.Enum;
using Livekit.Server.Sdk;
using Livekit.Server.Sdk.Dotnet;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LetsTalk.Services.Livekit
{
        // --- 2.1. Modèle de configuration ---
        // (Peut être dans un fichier séparé, mais inclus ici pour la complétude)
        public class LivekitSettings
        {
            public string ServerUrl { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
        }

        // --- 2.2. La Classe de Service Principale ---
        public class LivekitService
        {
            private readonly LivekitSettings _settings;
            private readonly RoomServiceClient _roomClient;
            private readonly IServiceScopeFactory _scopeFactory;

        // Le constructeur utilise l'injection de dépendance pour récupérer les configurations
            public LivekitService(IOptions<LivekitSettings> settings,IServiceScopeFactory scopeFactory)
            {
                _settings = settings.Value;
                _scopeFactory = scopeFactory;

            // Initialisation du client d'administration
            _roomClient = new RoomServiceClient(
                    _settings.ServerUrl,
                    _settings.ApiKey,
                    _settings.ApiSecret
                );
            }

            public async Task CreerSalonsInitiauxAsync()
            {
                // C'est déjà bien de créer un scope pour le DbContext dans un singleton.
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Vous aurez besoin d'un 'using Microsoft.Extensions.DependencyInjection;' en haut du fichier
                    var db = scope.ServiceProvider.GetRequiredService<LetsTalk.Context.AppDbContext>();

                    // Assurez-vous d'avoir un using System.Linq; pour .Where().ToList()
                    foreach (var canal in db.Canaux.Where(c => c.Type == ChannelType.Voice))
                    {
                        // ⚠️ On utilise le mot-clé await ici
                        await CreerNouveauSalon(canal.Nom, 120);
                    }
                }
            }

        // =======================================================
        // A. AUTHENTIFICATION : Génération de Jeton (Token)
        // =======================================================

        /// <summary>
        /// Génère un jeton JWT pour un client spécifique afin qu'il puisse rejoindre un salon.
        /// </summary>
        /// <param name="roomName">Le nom du salon à rejoindre.</param>
        /// <param name="identity">L'ID unique de l'utilisateur.</param>
        /// <returns>Le jeton JWT à fournir au client.</returns>
        public string GenerateAccessToken(string roomName, string identity)
            {
                // 1. Définir les autorisations
                var grant = new VideoGrants
                {
                    Room = roomName,// Nom du salon (autorise implicitement le join)
                    CanPublish = true,         // Peut publier de l'audio/vidéo
                    CanSubscribe = true,       // Peut s'abonner aux autres pistes
                    CanUpdateOwnMetadata = true,
                    RoomJoin = true
                };

                // 2. Créer le Jeton d'Accès
                var token = new AccessToken(_settings.ApiKey, _settings.ApiSecret)
                    .WithIdentity(identity)
                    .WithTtl(TimeSpan.FromHours(2)) // Valide pendant 2 heures
                    .WithGrants(grant);

                // 3. Retourner le JWT
                return token.ToJwt();
            }

            // =======================================================
            // B. ADMINISTRATION : Gestion des Salons (Rooms)
            // =======================================================

            /// <summary>
            /// Crée un nouveau salon avec des paramètres spécifiques.
            /// </summary>
            /// <param name="nomDuSalon">Nom unique du salon à créer.</param>
            /// <param name="dureeMaxMinutes">Durée de vie max du salon.</param>
            public async Task<Room> CreerNouveauSalon(string nomDuSalon, int dureeMaxMinutes)
            {
                var request = new CreateRoomRequest
                {
                    Name = nomDuSalon,
                    MaxParticipants = 50,
                    // Le salon se ferme 10 minutes (600s) après le départ du dernier participant
                    EmptyTimeout = 600,
                    MaxPlayoutDelay = (uint)(dureeMaxMinutes * 60)

                };

                // Utilise le RoomServiceClient pour créer la room sur le serveur LiveKit
                return await _roomClient.CreateRoom(request);
            }

            /// <summary>
            /// Récupère la liste de tous les salons actifs.
            /// </summary>
            public async Task<IEnumerable<Room>> ListerSalonsActifs()
            {
                var response = await _roomClient.ListRooms(new ListRoomsRequest());
                return response.Rooms;
            }

            /// <summary>
            /// Force la fermeture d'un salon.
            /// </summary>
            public async Task FermerSalon(string nomDuSalon)
            {
                var request = new DeleteRoomRequest { Room = nomDuSalon };
                await _roomClient.DeleteRoom(request);
                Console.WriteLine($"Salon {nomDuSalon} fermé avec succès.");
            }

        public async Task<List<ParticipantInfo>> GetVoiceMembersr(string roomName)
        {
            Debug.WriteLine("recuperation des membre");
            // Correction : utiliser la bonne méthode ListParticipants (et non ListeParticipantAsync)
            var response = await _roomClient.ListParticipants(new ListParticipantsRequest { Room = roomName });
            Debug.WriteLine(response.Participants.ToList());
            return response.Participants.ToList();
        }

     }
    }
