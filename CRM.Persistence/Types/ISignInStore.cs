using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface ISignInStore
  {
    Task<EntityUser> FindUserByEmail(string email);
    Task SaveToken(string email, string token);
  }
}
