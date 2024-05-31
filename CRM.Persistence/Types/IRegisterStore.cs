using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IRegisterStore
  {
    Task FindUserByEmail(string email);
    Task SaveNewUser(EntityUser newUser);
    Task RemoveUser(string email);
    Task ConfirmRegister(string email);
  }
}
