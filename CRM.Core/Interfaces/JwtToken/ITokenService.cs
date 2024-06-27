using CRM.Core.Enums;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.JwtToken
{
  public interface ITokenService
  {
    string GetJwtToken(User user, TokenTypes typesTokens);
    JwtTokenClaims TokenDecryption(string token);
    Task ValidateToken(string token);
  }
}