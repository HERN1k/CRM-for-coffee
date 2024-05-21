using CRM.Core.Models;

namespace CRM.Data.Types
{
  public interface IAuthRecoveryStore
  {
    Task<MainUser?> FindUserByEmail(string email);
    Task<bool> SaveNewPassword(int id, string hash);
  }
}
