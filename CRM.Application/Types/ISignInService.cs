using CRM.Application.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;

namespace CRM.Application.Types
{
    public interface ISignInService
  {
    Task<bool> SetData(string email);
    ValidationResult ValidationDataSignIn(SignInRequest request);
    bool VerificationHash(string requestPassword);
    string GetJwtToken(TypesToken typesTokens);
    Task<bool> SaveToken(string token);
    SignInResponse? SetResponse(string refreshToken);
    CookieOptions SetCookieOptions(TypesToken typesTokens);
  }
}
