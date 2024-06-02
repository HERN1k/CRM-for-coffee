using CRM.Core.Entities;

namespace CRM.Core.Interfaces.AuthRepository
{
  public interface IRegisterRepository
  {
    Task FindUserByEmail(string email);
    Task SaveNewUser(EntityUser newUser);
    Task RemoveUser(string email);
    Task ConfirmRegister(string email);
  }
}
