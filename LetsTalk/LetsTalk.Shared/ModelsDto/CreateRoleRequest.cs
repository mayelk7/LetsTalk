using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.ModelsDto
{
    public class CreateRoleRequest
    {
        public string Nom { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public int ServerId { get; set; }
        public long Permissions { get; set; } = 0;
    }
}
