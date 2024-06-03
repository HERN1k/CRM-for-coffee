using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IAuthSundryService
  {
    void ValidationEmail(string email);
    Task CheckImmutableToken(string email, string refreshToken);
    Task ValidateToken(string token);
    Task RemoveRefreshToken();
    string GetJwtAccessToken();
    Task ValidateDataUpdatePassword(UpdatePasswordRequest request);
    void VerificationPassword(string requestPassword);
    CookieOptions SetCookieOptions();
    string GetHash(string requestPassword);
    Task SaveNewPassword(string password);
    Task SendUpdatePasswordEmail();
  }
}
