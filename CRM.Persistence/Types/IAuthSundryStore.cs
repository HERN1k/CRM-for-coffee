using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IAuthSundryStore
  {
    Task<EntityUser> FindUserByEmail(string email);
    Task<string> FindTokenById(Guid id);
    Task RemoveRefreshToken(Guid id);
    Task SaveNewPassword(string email, string password);
  }
}
