using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IRegisterStore
  {
    Task<bool> FindUserByEmail(string email);
    Task<bool> SaveNewUser(EntityUser newUser);
    Task<bool> RemoveUser(string email);
    Task<bool> ConfirmRegister(string email);
  }
}
