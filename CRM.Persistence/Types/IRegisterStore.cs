using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IRegisterStore
  {
    Task<bool> FindUserByEmail(string email);
    Task<bool> SaveNewUser(User newUser);
    Task<bool> RemoveUser(string email);
    Task<bool> ConfirmRegister(string email);
  }
}
