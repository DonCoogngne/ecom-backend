namespace ecom_backend.DTOs.Security;

public class SecurityDto
{
    public bool TwoFactorEnabled { get; set; }
}

public class UpdateTwoFactorRequest
{
    public bool Enabled { get; set; }
}
