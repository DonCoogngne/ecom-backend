using System.ComponentModel.DataAnnotations;

namespace ecom_backend.DTOs.Profile;

public class UpdateProfileRequest
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; }
}
