using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.Perm
{

    [Flags]
    public enum ServerPermission : long
    {
        None = 0,
        SendMessage = 1 << 0,  // 1
        DeleteMessage = 1 << 1,  // 2
        KickMember = 1 << 2,  // 4
        BanMember = 1 << 3,  // 8
        ManageChannels = 1 << 4,  // 16
        ManageRoles = 1 << 5,  // 32
        ManageServer = 1 << 6,  // 64
        Administrator = 1 << 7,  // 128
    }
    
}
