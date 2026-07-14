using ecom_backend.Domain.Entities;
using ecom_backend.Models;

namespace ecom_backend.Mappings;

public static class EntityMappings
{
    public static RoleModel ToModel(this Role entity) => new()
    {
        RoleId = entity.RoleId,
        RoleName = entity.RoleName
    };

    public static UserModel ToModel(this User entity) => new()
    {
        UserId = entity.UserId,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash,
        ProfileImageUrl = entity.ProfileImageUrl,
        CreatedDate = entity.CreatedDate,
        IsActive = entity.IsActive,
        RoleId = entity.RoleId,
        GoogleId = entity.GoogleId,
        Role = entity.Role is null
            ? new RoleModel { RoleId = entity.RoleId }
            : entity.Role.ToModel()
    };

    public static User ToEntity(this UserModel model) => new()
    {
        UserId = model.UserId,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        PasswordHash = model.PasswordHash,
        ProfileImageUrl = model.ProfileImageUrl,
        CreatedDate = model.CreatedDate,
        IsActive = model.IsActive,
        RoleId = model.RoleId,
        GoogleId = model.GoogleId
    };

    public static void ApplyToEntity(this UserModel model, User entity)
    {
        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.Email = model.Email;
        entity.PasswordHash = model.PasswordHash;
        entity.ProfileImageUrl = model.ProfileImageUrl;
        entity.CreatedDate = model.CreatedDate;
        entity.IsActive = model.IsActive;
        entity.RoleId = model.RoleId;
        entity.GoogleId = model.GoogleId;
    }
}
