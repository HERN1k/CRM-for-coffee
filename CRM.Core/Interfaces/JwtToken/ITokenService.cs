using System.Security.Claims;

namespace CRM.Core.Interfaces.JwtToken
{
  public interface ITokenService
  {
    string CreateJwtToken(List<Claim> claims, int time);
    string TokenDecryption(string token);
    Task ValidateToken(string token);
  }
}