using System.ComponentModel.DataAnnotations;

namespace ecom_backend.DTOs.Business;

public class SaveBusinessProfileRequest
{
    [MaxLength(150)]
    public string? BusinessName { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(300)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(20)]
    public string? PrimaryColor { get; set; }

    [MaxLength(20)]
    public string? SecondaryColor { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }
}
