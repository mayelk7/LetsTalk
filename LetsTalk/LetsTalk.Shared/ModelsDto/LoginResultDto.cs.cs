using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.ModelsDto
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public int? UserId { get; set; }
        public string? Message { get; set; }
    }
}
