using System.Security.Claims;

namespace FirstAPI.interfaces
{
    public interface ITokenService
    {
        string CreateToken(List<Claim> claims);
    }
}