using Bogus;
using LetsTalk.Models;
using LetsTalk.Context;
using LetsTalk.Shared.Enum;

namespace LetsTalk.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // -------------------------
            // Utilisateurs
            // -------------------------
            if (!context.Utilisateurs.Any())
            {
                var faker = new Faker<Utilisateur>()
                    .RuleFor(u => u.Username, f => f.Internet.UserName())
                    .RuleFor(u => u.Email, f => f.Internet.Email())
                    .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("##########"))
                    .RuleFor(u => u.Password, f => "password123")
                    .RuleFor(u => u.ProfilPicture, f => f.Internet.Avatar())
                    .RuleFor(u => u.Actif, f => true)
                    .RuleFor(u => u.Salt, f => BitConverter.GetBytes(f.Random.Int()))
                    .RuleFor(u => u.CreatedAt, f => f.Date.Past())
                    .RuleFor(u => u.Type2Fa, f => "None");

                var utilisateurs = faker.Generate(10);
                context.Utilisateurs.AddRange(utilisateurs);
                context.SaveChanges();
            }

            var allUsers = context.Utilisateurs.ToList();

            // -------------------------
            // Serveurs
            // -------------------------
            if (!context.Servers.Any())
            {
                var servers = new List<Server>
                {
                    new Server { Nom = "Serveur Principal", OwnerId = allUsers[0].UtilisateurId },
                    new Server { Nom = "Serveur Secondaire", OwnerId = allUsers[1].UtilisateurId }
                };
                context.Servers.AddRange(servers);
                context.SaveChanges();
            }

            var allServers = context.Servers.ToList();

            // -------------------------
            // Canaux
            // -------------------------
            if (!context.Canaux.Any())
            {
                var channels = new List<Canaux>
                {
                    new Canaux { Nom = "Général", ServerId = allServers[0].ServerId, Type =  ChannelType.Text},
                    new Canaux { Nom = "Discussion vocale", ServerId = allServers[0].ServerId,Type = ChannelType.Voice },
                    new Canaux { Nom = "Général 2", ServerId = allServers[1].ServerId, Type = ChannelType.Text }
                };
                context.Canaux.AddRange(channels);
                context.SaveChanges();
            }

            var allCanaux = context.Canaux.ToList();

            // -------------------------
            // Rôles
            // -------------------------
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Nom = "Admin", Level = 100, ServerId = allServers[0].ServerId },
                    new Role { Nom = "Modérateur", Level = 50, ServerId = allServers[0].ServerId },
                    new Role { Nom = "Membre", Level = 10, ServerId = allServers[0].ServerId }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            var allRoles = context.Roles.ToList();

            // -------------------------
            // Membres (Utilisateur + Serveur + Role)
            // -------------------------
            if (!context.Membres.Any())
            {
                var membres = new List<Membre>
                {
                    new Membre { UtilisateurId = allUsers[0].UtilisateurId, ServerId = allServers[0].ServerId, RoleId = allRoles.First().RoleId },
                    new Membre { UtilisateurId = allUsers[1].UtilisateurId, ServerId = allServers[0].ServerId, RoleId = allRoles.Last().RoleId },
                    new Membre { UtilisateurId = allUsers[2].UtilisateurId, ServerId = allServers[1].ServerId, RoleId = allRoles.Last().RoleId }
                };
                context.Membres.AddRange(membres);
                context.SaveChanges();
            }

            // -------------------------
            // Messages dans Canaux
            // -------------------------
            if (!context.MessagesCanal.Any())
            {
                var messages = new List<MessageCanal>
                {
                    new MessageCanal { Contenu = "Salut tout le monde !", DateEnvoi = DateTime.Now.AddMinutes(-30), Epingle = false, UtilisateurId = allUsers[0].UtilisateurId, CanalId = allCanaux[0].CanauxId },
                    new MessageCanal { Contenu = "Bienvenue sur le serveur", DateEnvoi = DateTime.Now.AddMinutes(-20), Epingle = false, UtilisateurId = allUsers[1].UtilisateurId, CanalId = allCanaux[0].CanauxId }
                };
                context.MessagesCanal.AddRange(messages);
                context.SaveChanges();
            }

            // -------------------------
            // Conversations privées et membres
            // -------------------------
            if (!context.ConversationPrivers.Any())
            {
                var conv = new ConversationPriver { CreatedAt = DateTime.Now };
                context.ConversationPrivers.Add(conv);
                context.SaveChanges();

                var convMembers = new List<MembreMP>
                {
                    new MembreMP { UtilisateurId = allUsers[0].UtilisateurId, ConversationId = conv.ConversationPriverId },
                    new MembreMP { UtilisateurId = allUsers[1].UtilisateurId, ConversationId = conv.ConversationPriverId }
                };
                context.MembreMPs.AddRange(convMembers);
                context.SaveChanges();

                // Message privé
                var mp = new MessagePriver
                {
                    Contenu = "Salut en privé !",
                    DateEnvoi = DateTime.Now,
                    Epingle = false,
                    UtilisateurId = allUsers[0].UtilisateurId,
                    ConversationPriverId = conv.ConversationPriverId
                };
                context.MessagesPriver.Add(mp);
                context.SaveChanges();
            }

            // -------------------------
            // Fichiers
            // -------------------------
            if (!context.Fichiers.Any())
            {
                var fichier = new Fichier
                {
                    Nom = "image_test.png",
                    Url = "/images/image_test.png",
                    Type = "image",
                    MessageType = MessageType.Canal,
                    MessageId = context.MessagesCanal.First().MessageId
                };
                context.Fichiers.Add(fichier);
                context.SaveChanges();
            }

            // -------------------------
            // Notifications
            // -------------------------
            if (!context.Notifications.Any())
            {
                var notification = new Notification
                {
                    UtilisateurId = allUsers[1].UtilisateurId,
                    MessageType = MessageType.Canal,
                    MessageId = context.MessagesCanal.First().MessageId,
                    Lu = false
                };
                context.Notifications.Add(notification);
                context.SaveChanges();
            }

            // -------------------------
            // Messages lus
            // -------------------------
            if (!context.MessageLus.Any())
            {
                var messagelus = new MessageLu
                {
                    UtilisateurId = allUsers[1].UtilisateurId,
                    MessageType = MessageType.Canal,
                    MessageId = context.MessagesCanal.First().MessageId,
                    Lu = false
                };
                context.MessageLus.Add(messagelus);
                context.SaveChanges();
            }
        }
    }
}

