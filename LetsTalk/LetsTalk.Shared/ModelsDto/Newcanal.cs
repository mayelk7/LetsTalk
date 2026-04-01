using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsTalk.Shared.Enum;

namespace LetsTalk.Shared.ModelsDto
{
    public record CreatecanalDto(int serverid, string Nom, ChannelType Type );
}
