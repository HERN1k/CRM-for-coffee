using CRM.Core.Contracts.RestDto;

namespace CRM.Application.Types
{
    public interface IAuthRecoveryService
  {
    ValidationResult ValidationDataRcoveryPass(RecoveryPasswordRequest request);
    Task<bool> СomparisonRecoveryPassData();
    string GetNewPassword(int length);
    Task<bool> SendRecoveryPassEmail(string password);
    Task<bool> SaveNewPassword(string password);
    Task<bool> RemoveRefreshToken();
  }
}
