namespace ecom_backend.Domain.Entities;

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? DeviceName { get; set; }

    // Computed column in SQL Server; read-only from the app.
    public bool IsActive { get; private set; }

    public User User { get; set; } = null!;
}
