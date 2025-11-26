using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LetsTalk.Data;
using LetsTalk.Models;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LetsTalkController : ControllerBase
    {
        private readonly BackApiEf _db;

        public LetsTalkController(BackApiEf db)
        {
            _db = db;
        }
        
        [HttpGet("GetAllMessageCanal")]
        public List<MessageCanalDto> GetAllMessageCanal()
        {
            return _db.GetAllMessagescanal();
        }

        [HttpGet("GetAllMessagePriver")]
        public List<MessagePriverDto> GetAllMessagepriver()
        {
            return _db.GetAllMessagespriver();
        }
        
        [HttpGet("GetAllGroupe")]
        public List<Canaux> GetAllGroupe()
        {
            return _db.GetAllSalons();
        }
        /*
        [HttpGet("GetAllMessageNonLus")]
        public void GetAllMessageNonLus(int Userid)
        {
            _db.GetAllMessagesPriverNonLus(Userid);
        }
        */
        [HttpGet("GetAllUser")]
        public List<Utilisateur> GetAllUser()
        {
            return _db.GetAllUsers();
        }

        [HttpGet("GetUser/{id}")]
        public Utilisateur GetUser(int id)
        {
            return _db.GetUserByID(id);
        }

        [HttpPost("SetNewUser")]
        public bool SetNewUser(string token, string username, string email, string phone, string password, string type2fa)
        {
            return _db.SetNewUser(token, username, email, phone, password, type2fa);
        }
       
        [HttpPost("SetNewGroupe")]
        public bool SetNewGroupe(string token, string nomsalon, int idOwner)
        {
            return _db.SetNewServer(token, nomsalon, idOwner);
        }
       
       [HttpPost("NewMessageCanal")]
       public bool NewMessageCanal(int iduser, string contenue, int canalId)
       {
           return _db.NewMessageCanal(iduser, contenue, canalId);
       }
        
        [HttpPost("NewMessagePriver")]
        public bool NewMessagePriver(int iduser, string contenue, int id_discution)
        {
            return _db.NewMessagePriver(iduser, id_discution, contenue);
        }
        /*
        [HttpPatch("UpdateGroupe/{id_discution}")]
        public void UpdateGroupe(int token, int id_discution)
        {
            // à remplir
        }

        [HttpPatch("UpdateUser/{id_user}")]
        public void UpdateUser(int token, int id_discution)
        {
            // à remplir
        }

        [HttpDelete("DeleteUser/{id_user}")]
        public void DeleteUser(int token, int id_user)
        {
            // à remplir
        }

        [HttpDelete("DeleteMessage/{id_discution}/{id_message}")]
        public void DeleteMessage(int token, int id_user)
        {
            // à remplir
        }

        [HttpDelete("DeleteGroupe/{id_discution}")]
        public void DeleteGroupe(int token, int id_user)
        {
            // à remplir
        }
        */
    }
}
