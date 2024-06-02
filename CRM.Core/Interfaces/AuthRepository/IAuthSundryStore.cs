using CRM.Core.Entities;

namespace CRM.Core.Interfaces.AuthRepository
{
  public interface IAuthSundryRepository
  {
    Task<EntityUser> FindUserByEmail(string email);
    Task<string> FindTokenById(Guid id);
    Task RemoveRefreshToken(Guid id);
    Task SaveNewPassword(string email, string password);
  }
}
