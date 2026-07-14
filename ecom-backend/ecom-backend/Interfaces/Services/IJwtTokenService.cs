using ecom_backend.Models;

namespace ecom_backend.Interfaces.Services;

public interface IJwtTokenService
{
    string CreateToken(UserModel user);
}
