namespace ecom_backend.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool TwoFactorEnabled { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string? GoogleId { get; set; }
}
