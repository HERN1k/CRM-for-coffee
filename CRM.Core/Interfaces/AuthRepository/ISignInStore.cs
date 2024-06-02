using CRM.Core.Entities;

namespace CRM.Core.Interfaces.AuthRepository
{
  public interface ISignInRepository
  {
    Task<EntityUser> FindUserByEmail(string email);
    Task SaveToken(string email, string token);
  }
}
