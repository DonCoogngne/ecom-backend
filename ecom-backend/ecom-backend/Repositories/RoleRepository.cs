using ecom_backend.Data;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class RoleRepository(AppDbContext db) : IRoleRepository
{
    public async Task<RoleModel?> GetByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.Roles
            .FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);

        return entity?.ToModel();
    }

    public async Task<RoleModel?> GetByIdAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);

        return entity?.ToModel();
    }
}
