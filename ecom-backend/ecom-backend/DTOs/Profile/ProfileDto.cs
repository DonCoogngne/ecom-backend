namespace ecom_backend.DTOs.Profile;

public class ProfileDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool GoogleConnected { get; set; }
    public bool HasPassword { get; set; }
}
