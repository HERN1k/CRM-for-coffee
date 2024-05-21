using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IAuthSundryStore
  {
    Task<User?> FindUserByEmail(string email);
    Task<string> FindTokenById(int id);
    Task<bool> RemoveRefreshToken(int id);
    Task<bool> SaveNewPassword(string email, string password);
  }
}
