namespace ecom_backend.Models;

public class BusinessProfileModel
{
    public int BusinessProfileId { get; set; }
    public int UserId { get; set; }
    public string? BusinessName { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? Location { get; set; }
}
