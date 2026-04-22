using LetsTalk.Context;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LetsTalk.Client.Views.Components.ServerSettingsDialog;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/role")]
public class RoleApiController(AppDbContext context) : BaseApiController
{

    public class AssignRoleRequest
    {
        public int RoleId { get; set; }
    }

    // ─── GET /api/role/server/{serverId} ─────────────────────────────────────
    [HttpGet("server/{serverId:int}")]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAllRoles(int serverId)
    {
        var roles = await context.Roles
            .Where(r => r.ServerId == serverId)
            .Select(r => new RoleDto(r.RoleId, r.Nom, r.Level, r.ServerId, r.Permissions))
            .ToListAsync();

        return Ok(Response("Rôles récupérés avec succès", roles));
    }

    // ─── GET /api/role/{roleId} ───────────────────────────────────────────────
    [HttpGet("{roleId:int}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(int roleId)
    {
        var role = await context.Roles.FindAsync(roleId);

        if (role == null)
            return NotFound(new ApiResponse<RoleDto>(false, "Rôle introuvable", null));

        return Ok(new ApiResponse<RoleDto>(true, "Rôle récupéré", new RoleDto(role.RoleId, role.Nom, role.Level, role.ServerId, role.Permissions)));
    }

    // ─── GET /api/role/user/{userId}/server/{serverId} ────────────────────────
    [HttpGet("user/{userId:int}/server/{serverId:int}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleByUserId(int userId, int serverId)
    {
        var membre = await context.Membres
            .Include(m => m.Role)
            .FirstOrDefaultAsync(m => m.UtilisateurId == userId && m.ServerId == serverId);

        if (membre == null)
            return NotFound(new ApiResponse<RoleDto>(false, "Membre introuvable", null));

        var r = membre.Role;
        return Ok(new ApiResponse<RoleDto>(true, "Rôle récupéré", new RoleDto(r.RoleId, r.Nom, r.Level, r.ServerId, r.Permissions)));
    }

    // ─── POST /api/role ───────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleRequest request)
    {
        Console.WriteLine($"nom : {request.Nom},level :{request.Level},serverid : {request.ServerId}, permissions : {request.Permissions}");

        var serverExists = await context.Servers.AnyAsync(s => s.ServerId == request.ServerId);
        if (!serverExists)
            return NotFound(new ApiResponse<bool>(false, "Serveur introuvable", false));

        var role = new Role
        {
            Nom = request.Nom,
            Level = request.Level,
            ServerId = request.ServerId,
            Permissions = request.Permissions
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();

        return Ok(new ApiResponse<bool>(true, "Rôle créé", true));
    }

    // ─── PUT /api/role/user/{userId}/server/{serverId} ────────────────────────
    [HttpPut("user/{userId:int}/server/{serverId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoleToUser(int userId, int serverId, [FromBody] AssignRoleRequest request)
    {
        var membre = await context.Membres
            .FirstOrDefaultAsync(m => m.UtilisateurId == userId && m.ServerId == serverId);

        if (membre == null)
            return NotFound(new ApiResponse<bool>(false, "Membre introuvable", false));

        var roleExists = await context.Roles.AnyAsync(r => r.RoleId == request.RoleId && r.ServerId == serverId);
        if (!roleExists)
            return BadRequest(new ApiResponse<bool>(false, "Rôle invalide pour ce serveur", false));

        context.Membres.Remove(membre);
        await context.SaveChangesAsync();

        context.Membres.Add(new Membre
        {
            UtilisateurId = userId,
            ServerId = serverId,
            RoleId = request.RoleId
        });
        await context.SaveChangesAsync();

        return Ok(new ApiResponse<bool>(true, "Rôle assigné avec succès", true));
    }

    // ─── DELETE /api/role/{roleId} ────────────────────────────────────────────
    [HttpDelete("{roleId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveRole(int roleId)
    {
        var role = await context.Roles
            .Include(r => r.Membres)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role == null)
            return NotFound(new ApiResponse<bool>(false, "Rôle introuvable", false));

        var fallbackRole = await context.Roles
            .FirstOrDefaultAsync(r => r.ServerId == role.ServerId && r.RoleId != roleId);

        if (fallbackRole == null)
            return BadRequest(new ApiResponse<bool>(false, "Impossible de supprimer le dernier rôle du serveur", false));

        foreach (var membre in role.Membres)
            membre.RoleId = fallbackRole.RoleId;

        await context.SaveChangesAsync();

        context.Roles.Remove(role);
        await context.SaveChangesAsync();

        return Ok(new ApiResponse<bool>(true, "Rôle supprimé avec succès", true));
    }

    // ─── DELETE /api/role/user/{userId}/server/{serverId} ────────────────────
    [HttpDelete("user/{userId:int}/server/{serverId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveRoleFromUser(int userId, int serverId)
    {
        var membre = await context.Membres
            .FirstOrDefaultAsync(m => m.UtilisateurId == userId && m.ServerId == serverId);

        if (membre == null)
            return NotFound(new ApiResponse<bool>(false, "Membre introuvable", false));

        var defaultRole = await context.Roles
            .Where(r => r.ServerId == serverId)
            .OrderBy(r => r.Level)
            .FirstOrDefaultAsync();

        if (defaultRole == null)
            return BadRequest(new ApiResponse<bool>(false, "Aucun rôle par défaut trouvé", false));

        context.Membres.Remove(membre);
        await context.SaveChangesAsync();

        context.Membres.Add(new Membre
        {
            UtilisateurId = userId,
            ServerId = serverId,
            RoleId = defaultRole.RoleId
        });
        await context.SaveChangesAsync();

        return Ok(new ApiResponse<bool>(true, "Rôle retiré avec succès", true));
    }
    // --- Put Roleid/Permission *------------------
    [HttpPut("{roleId:int}/permissions")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePermissions(int roleId, [FromBody] UpdateRolePermissionsRequest request)
    {
        var role = await context.Roles.FindAsync(roleId);
        if (role == null)
            return NotFound(new ApiResponse<bool>(false, "Rôle introuvable", false));

        role.Permissions = request.Permissions;
        await context.SaveChangesAsync();

        return Ok(new ApiResponse<bool>(true, "Permissions mises à jour", true));
    }

}