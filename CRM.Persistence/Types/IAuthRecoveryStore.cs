using CRM.Core.Models;

namespace CRM.Data.Types
{
  public interface IAuthRecoveryStore
  {
    Task<User> FindUserByEmail(string email);
    Task SaveNewPassword(Guid id, string hash);
    Task RemoveRefreshToken(Guid id);
  }
}
