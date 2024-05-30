using CRM.Core.Models;

namespace CRM.Data.Types
{
  public interface IAuthRecoveryStore
  {
    Task<User?> FindUserByEmail(string email);
    Task<bool> SaveNewPassword(Guid id, string hash);
    Task<bool> RemoveRefreshToken(Guid id);
  }
}
