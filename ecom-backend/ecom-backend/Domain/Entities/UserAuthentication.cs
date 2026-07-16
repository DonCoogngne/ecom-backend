namespace ecom_backend.Domain.Entities;

public class UserAuthentication
{
    public int UserAuthenticationId { get; set; }
    public int UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ProviderUserId { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }

    public User User { get; set; } = null!;
}
