using CRM.Core.Models;

namespace CRM.Core.Interfaces.AuthRepository
{
  public interface IAuthRecoveryRepository
  {
    Task<User> FindUserByEmail(string email);
    Task SaveNewPassword(Guid id, string hash);
    Task RemoveRefreshToken(Guid id);
  }
}