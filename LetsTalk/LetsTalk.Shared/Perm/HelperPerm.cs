using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsTalk.Shared.Perm
{
    public class HelperPerm
    {
        public static bool HasPermission(long rolePermissions, ServerPermission permission)
        {
            var perms = (ServerPermission)rolePermissions;
            if (perms.HasFlag(ServerPermission.Administrator)) return true;
            return perms.HasFlag(permission);
        }
    }
}
