using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IAuthSundryStore
  {
    Task<User?> FindUserByEmail(string email);
    Task<string> FindTokenById(Guid id);
    Task<bool> RemoveRefreshToken(Guid id);
    Task<bool> SaveNewPassword(string email, string password);
  }
}
