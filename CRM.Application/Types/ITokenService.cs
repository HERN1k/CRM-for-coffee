using System.Security.Claims;

namespace CRM.Application.Types
{
  public interface ITokenService
  {
    string CreateJwtToken(List<Claim> claims, int time);
    string TokenDecryption(string token);
    Task<bool> ValidateToken(string token);
  }
}
