using DataAccess.Entities;

namespace DataAccess.JwtHandler
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, string roles);
    }
}
