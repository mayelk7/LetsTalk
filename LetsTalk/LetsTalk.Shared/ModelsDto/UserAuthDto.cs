using LetsTalk.Shared.Perm;

namespace LetsTalk.Shared.ModelsDto
{
    public record UserAuthDto(
    int UtilisateurId,
    string Username,
    string Email,
    string Phone,
    string? ProfilPicture,
    bool Actif,
    DateTime CreatedAt,
    List<int> OwnedServerIds,
    Dictionary<int, int> ServerRoles,
    Dictionary<int, long> ServerPermissions
)
    {
        public bool HasPermission(int serverId, ServerPermission permission)
        {
            if (OwnedServerIds.Contains(serverId)) return true;
            if (!ServerPermissions.TryGetValue(serverId, out var perms)) return false;
            return HelperPerm.HasPermission(perms, permission);
        }
    }

}


