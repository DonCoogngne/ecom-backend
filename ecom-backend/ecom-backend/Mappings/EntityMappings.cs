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
        Phone = entity.Phone,
        ProfileImageUrl = entity.ProfileImageUrl,
        CreatedDate = entity.CreatedDate,
        IsActive = entity.IsActive,
        TwoFactorEnabled = entity.TwoFactorEnabled,
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
        Phone = model.Phone,
        ProfileImageUrl = model.ProfileImageUrl,
        CreatedDate = model.CreatedDate,
        IsActive = model.IsActive,
        TwoFactorEnabled = model.TwoFactorEnabled,
        RoleId = model.RoleId,
        GoogleId = model.GoogleId
    };

    public static void ApplyToEntity(this UserModel model, User entity)
    {
        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.Email = model.Email;
        entity.PasswordHash = model.PasswordHash;
        entity.Phone = model.Phone;
        entity.ProfileImageUrl = model.ProfileImageUrl;
        entity.CreatedDate = model.CreatedDate;
        entity.IsActive = model.IsActive;
        entity.TwoFactorEnabled = model.TwoFactorEnabled;
        entity.RoleId = model.RoleId;
        entity.GoogleId = model.GoogleId;
    }

    public static BusinessProfileModel ToModel(this BusinessProfile entity) => new()
    {
        BusinessProfileId = entity.BusinessProfileId,
        UserId = entity.UserId,
        BusinessName = entity.BusinessName,
        Category = entity.Category,
        Description = entity.Description,
        Website = entity.Website,
        LogoUrl = entity.LogoUrl,
        PrimaryColor = entity.PrimaryColor,
        SecondaryColor = entity.SecondaryColor,
        Location = entity.Location
    };

    public static SubscriptionModel ToModel(this Subscription entity) => new()
    {
        SubscriptionId = entity.SubscriptionId,
        UserId = entity.UserId,
        PlanName = entity.PlanName,
        FreeCredits = entity.FreeCredits,
        RemainingCredits = entity.RemainingCredits,
        Invoices = entity.Invoices
            .OrderByDescending(i => i.InvoiceDate)
            .Select(i => i.ToModel())
            .ToList()
    };

    public static InvoiceModel ToModel(this Invoice entity) => new()
    {
        InvoiceId = entity.InvoiceId,
        InvoiceNumber = entity.InvoiceNumber,
        InvoiceDate = entity.InvoiceDate,
        Amount = entity.Amount,
        Status = entity.Status
    };
}
