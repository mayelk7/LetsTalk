using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using LetsTalk.Context;
using LetsTalk.Models;
using LetsTalk.Shared;
namespace LetsTalk.Data;
public class BackApiEf
{
    private readonly AppDbContext _db;

    public BackApiEf(AppDbContext db)
    {
        _db = db;
    }

    // Récupérer tous les utilisateurs
    public List<Utilisateur> GetAllUsers()
    {
        return _db.Utilisateurs.ToList();
    }

    // Récupérer un utilisateur par ID
    public Utilisateur GetUserByID(int id)
    {
        return _db.Utilisateurs
                  .FirstOrDefault(u => u.UtilisateurId == id);
    }

    // Créer un nouvel utilisateur
    public bool SetNewUser(string token, string username, string email, string phone, string password, string type2fa)
    {
        if (!isAdmin(token))
            throw new UnauthorizedAccessException("Seul un administrateur peut créer un nouvel utilisateur.");

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);

        var user = new Utilisateur
        {
            Username = username,
            Email = email,
            Phone = phone,
            Password = Convert.ToBase64String(pbkdf2.GetBytes(32)), // Stocker la clé dérivée en Base64
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
    public List<MessageCanalDto> GetAllMessagescanal()
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

    public List<MessagePriverDto> GetAllMessagespriver()
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
        if (!isAdmin(token))
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
            Type = ChannelType.texte
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
    public bool isAdmin(string token)
    {
        return token == "serveradmin";
    }
}
