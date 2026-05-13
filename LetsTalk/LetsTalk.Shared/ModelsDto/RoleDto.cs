using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.ModelsDto
{
    public record RoleDto(int RoleId, string Nom, int Level, int ServerId, long Permissions);

}
