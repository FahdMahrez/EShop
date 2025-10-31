using EShop.Data;

namespace EShop.Services.Interface
{
    public interface ITokenService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
        string GenerateRefreshToken();
    }
}
