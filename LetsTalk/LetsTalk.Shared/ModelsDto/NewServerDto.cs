using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.ModelsDto
{
     public record CreateServerDto(string Nom, int OwnerId, List<int>? Membres);
}
