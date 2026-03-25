using Microsoft.AspNetCore.Mvc;
using LetsTalk.Data;
using LetsTalk.Models;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LetsTalkController : ControllerBase
    {
        private readonly BackApiEf _db;

        public LetsTalkController(BackApiEf db)
        {
            _db = db;
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
