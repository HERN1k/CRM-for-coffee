using CRM.Core.Contracts.RestDto;
using Microsoft.AspNetCore.Http;

namespace CRM.Application.Types
{
    public interface IAuthSundryService
  {
    bool ValidationEmail(string email);
    Task<bool> CheckImmutableToken(string email, string refreshToken);
    Task<bool> ValidateToken(string token);
    Task<bool> RemoveRefreshToken();
    string GetJwtAccessToken();
    ValidationResult ValidateDataUpdatePassword(UpdatePasswordRequest request);
    Task<bool> SetDataUpdatePassword(UpdatePasswordRequest request);
    bool VerificationPassword(string requestPassword);
    CookieOptions SetCookieOptions();
    string GetHash(string requestPassword);
    Task<bool> SaveNewPassword(string password);
    Task<bool> SendUpdatePasswordEmail();
  }
}
