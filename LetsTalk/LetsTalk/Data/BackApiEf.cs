using Bogus.Extensions.Italy;
using LetsTalk.Context;
using LetsTalk.Helpers;
using LetsTalk.Models;
using LetsTalk.Services.Livekit;
using LetsTalk.Shared;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;
using Livekit.Client;
using Livekit.Server.Sdk;
using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using static Livekit.Server.Sdk.Dotnet.ParticipantInfo.Types;

namespace LetsTalk.Data;
public class BackApiEf
{
    private readonly AppDbContext _db;
    private readonly LivekitService _livekit;

    public BackApiEf(AppDbContext db, LivekitService livekit)
    {
        _db = db;
        _livekit = livekit;
    }

    // Récupérer tous les utilisateurs
    public List<Utilisateur> GetAllUsers()
    {
        return _db.Utilisateurs.ToList();
    }

    // Récupérer un utilisateur par ID
    public Utilisateur? GetUserById(int id)
    {
        return _db.Utilisateurs
                  .FirstOrDefault(u => u.UtilisateurId == id);
    }

    // Créer un nouvel utilisateur
    public bool SetNewUser(string token, string username, string email, string phone, string password, string type2fa)
    {
        if (!IsAdmin(token))
            throw new UnauthorizedAccessException("Seul un administrateur peut créer un nouvel utilisateur.");

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);

        var user = new Utilisateur
        {
            Username = username,
            Email = email,
            Phone = phone,
            Password = PasswordHelper.Hash(password),
            ProfilPicture = null,
            Actif = true,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
            Type2Fa = type2fa
        };

        _db.Utilisateurs.Add(user);
        return _db.SaveChanges() > 0;
    }

    // Récupérer tous les messages
    public List<MessageCanalDto> GetAllMessagesCanal()
    {
        return _db.MessagesCanal
            .Include(m => m.Utilisateur)
            .Include(m => m.Canal)
            .Select(m => new MessageCanalDto
            {
                MessageId = m.MessageId,
                Contenu = m.Contenu,
                DateEnvoi = m.DateEnvoi,
                UtilisateurId = m.UtilisateurId,
                Username = m.Utilisateur.Username,
                CanalId = m.CanalId,
                NomCanal = m.Canal.Nom
            })
            .ToList();
    }

    public List<MessagePriverDto> GetAllMessagesPriver()
    {
        return _db.MessagesPriver
            .Include(m => m.Utilisateur)
            .Include(m => m.ConversationPriver)
                .ThenInclude(c => c.MembreMPs)
                    .ThenInclude(mp => mp.Utilisateur)
            .Select(m => new MessagePriverDto
            {
                MessageId = m.MessagePriverId,
                Contenu = m.Contenu,
                DateEnvoi = m.DateEnvoi,

                UtilisateurId = m.UtilisateurId,
                Username = m.Utilisateur.Username,

                ConversationPriverId = m.ConversationPriverId,

                MembreMPs = m.ConversationPriver.MembreMPs
                    .Select(mp => new MembreMPDto
                    {
                        UtilisateurId = mp.UtilisateurId,
                        Username = mp.Utilisateur.Username
                    })
                    .ToList()
            })
            .ToList();
    }

    // Récupérer tous les salons
    public List<Canaux> GetAllSalons()
    {
        return _db.Canaux.ToList();
    }

    // Créer un nouveau message
    public bool NewMessageCanal(int idUser, string contenu, int canalId)
    {
        var message = new MessageCanal
        {
            Contenu = contenu,
            DateEnvoi = DateTime.UtcNow,
            Epingle = false,
            UtilisateurId = idUser,
            CanalId = canalId
        };

        _db.MessagesCanal.Add(message);
        return _db.SaveChanges() > 0;
    }

    public bool NewMessagePriver(int idUser, int id_discution, string contenu)
    {
        var message = new MessagePriver
        {
            Contenu = contenu,
            DateEnvoi = DateTime.UtcNow,
            Epingle = false,
            UtilisateurId = idUser,
            ConversationPriverId = id_discution,
        };

        _db.MessagesPriver.Add(message);
        return _db.SaveChanges() > 0;
    }

    // Créer un nouveau salon + canal
    public bool SetNewServer(string token, string nomSalon, int idOwner)
    {
        if (!IsAdmin(token))
            throw new UnauthorizedAccessException("Seul un administrateur peut créer un nouveau salon.");

        var server = new Server { Nom = nomSalon,OwnerId = idOwner};
        _db.Servers.Add(server);
        if (_db.SaveChanges() == 0)
        {
            return false;
        }
        var canal = new Canaux
        {
            Nom = "Général",
            ServerId = server.ServerId,
            Type = ChannelType.Text
        };

        _db.Canaux.Add(canal);
        return _db.SaveChanges() > 0;
    }

    // Messages non lus pour un utilisateur
    public List<MessagePriver> GetAllMessagesPriverNonLus(int idUser)
    {
        return _db.MessagesPriver
                  .Where(m => !_db.MessageLus.Any(ml => ml.MessageId == m.MessagePriverId && ml.UtilisateurId == idUser))
                  .ToList();
    }
    public List<MessageCanal> GetAllMessagesCanalNonLus(int idUser)
    {
        return _db.MessagesCanal
                  .Where(m => !_db.MessageLus.Any(ml => ml.MessageId == m.MessageId && ml.UtilisateurId == idUser))
                  .ToList();
    }
    // Vérification administrateur
    public bool IsAdmin(string token)
    {
        return token == "serveradmin";
    }
    
    /// <summary>
    ///     Get all servers record
    /// </summary>
    ///
    /// <returns> List of <c>Server</c>  </returns>
    public List<Server> GetAllServers()
    {
        return _db.Servers.ToList();
    }
    
    /// <summary>
    ///     Retrieve a server by its ID
    /// </summary>
    /// 
    /// <param name="serverId"></param>
    ///
    /// <returns> The <c>Server</c> corresponding to the Id or null </returns>
    public Server? GetServerById(int serverId)
    {
        return _db.Servers
            .Include(s => s.Canaux)
                .ThenInclude(c => c.Messages)
            .Include(s => s.Membres)
                .ThenInclude(m => m.Utilisateur)
            .FirstOrDefault(s => s.ServerId == serverId);
    }
    
    
    /// <summary>
    ///    Retrieve all servers linked to a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>
    ///      List of <c>Server</c> representing the servers associated with the specified user ID
    /// </returns>
    public List<Server> GetUserServers(int userId)
    {
        return _db.Servers
            .Where(s => s.Membres.Any(m => m.UtilisateurId == userId))
            .ToList();
    }

    public async Task<List<ParticipantLiveKit>> GetMembersWithDbInfo(string roomName)
    {
        // 1️⃣ Récupérer les participants LiveKit
        var participants = await _livekit.GetVoiceMembersr(roomName);
        Debug.WriteLine("participant recupere");
        // 2️⃣ Créer la liste finale avec infos DB
        var allParticipants = new List<ParticipantLiveKit>();
        foreach (var p in participants)
        {
            Debug.WriteLine("premier menbre :", p.Identity);

            var success = int.TryParse(p.Identity, out var identityId);

            if (!success)
            {
                throw new InvalidOperationException($"Impossible de récupérer ou caster en id l'ID pour le participant LiveKit : {p.Identity}");
            }

            // Si on arrive ici, identityId est valide
            var userFromDb = GetUserById(identityId); // ou await si async
                                                      // 3️⃣ Récupérer les infos de ton serveur / BDD
            if (userFromDb == null)
                throw new InvalidOperationException($"Aucun utilisateur trouvé en base pour l'ID {identityId}");


            // 4️⃣ Créer ton objet final
            var utilisateur = new ParticipantLiveKit
                (
                    identityId,
                    p.Name,
                    p.State.ToString(),
                    p.Permission?.CanPublish ?? false,
                    p.Permission?.CanSubscribe ?? false,
                    // Ajouter les infos D
                    userFromDb.ProfilPicture,
                    userFromDb.Username

            );

            allParticipants.Add(utilisateur);
        }

        return allParticipants;
    }


}