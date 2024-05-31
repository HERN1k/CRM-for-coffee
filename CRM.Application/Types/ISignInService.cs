using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;

namespace CRM.Application.Types
{
  public interface ISignInService
  {
    Task SetData(string email);
    void ValidationDataSignIn(SignInRequest request);
    void VerificationHash(string requestPassword);
    string GetJwtToken(TokenTypes typesTokens);
    Task SaveToken(string token);
    SignInResponse SetResponse(string refreshToken);
    CookieOptions SetCookieOptions(TokenTypes typesTokens);
  }
}
