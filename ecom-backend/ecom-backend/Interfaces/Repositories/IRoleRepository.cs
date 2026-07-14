using ecom_backend.Models;

namespace ecom_backend.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<RoleModel?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);

    Task<RoleModel?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default);
}
