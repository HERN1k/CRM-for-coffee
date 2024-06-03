using CRM.Core.Contracts.RestDto;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IAuthRecoveryService
  {
    Task ValidationDataRecoveryPass(RecoveryPasswordRequest request);
    void СomparisonRecoveryPassData(RecoveryPasswordRequest request);
    string GetNewPassword(int length);
    Task SendRecoveryPassEmail(string password);
    Task SaveNewPassword(string password);
    Task RemoveRefreshToken();
  }
}
