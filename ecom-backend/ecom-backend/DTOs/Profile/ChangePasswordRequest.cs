using System.ComponentModel.DataAnnotations;

namespace ecom_backend.DTOs.Profile;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }

    [Required, MinLength(8), MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
