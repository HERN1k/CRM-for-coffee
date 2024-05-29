using CRM.API.Contarcts;

namespace CRM.Application.Types
{
  public interface IRegisterService
  {
    bool AddToModel(RegisterRequest request);
    ValidationResult ValidationDataRegister();
    Task<bool> UserIsRegister();
    bool GetHash();
    Task<bool> SaveNewUser();
    Task<bool> RemoveUser();
    Task<bool> SendEmailConfirmRegister();
    bool FromBase64ToString(string input);
    bool ValidationEmail();
    Task<bool> ConfirmRegister();
  }
}
