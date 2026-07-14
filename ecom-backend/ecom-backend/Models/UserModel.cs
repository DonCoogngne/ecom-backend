namespace ecom_backend.Models;

public class UserModel
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }
    public string? GoogleId { get; set; }
    public RoleModel Role { get; set; } = new();
}
