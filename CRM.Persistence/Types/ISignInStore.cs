using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface ISignInStore
  {
    Task<User?> FindUserByEmail(string email);
    Task<bool> SaveToken(string email, string token);
  }
}
