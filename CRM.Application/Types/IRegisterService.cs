using CRM.Core.Contracts.RestDto;

namespace CRM.Application.Types
{
  public interface IRegisterService
  {
    void AddToModel(RegisterRequest request);
    void ValidationDataRegister();
    Task UserIsRegister();
    void GetHash();
    Task SaveNewUser();
    Task RemoveUser();
    Task SendEmailConfirmRegister();
    void FromBase64ToString(string input);
    void ValidationEmail();
    Task ConfirmRegister();
  }
}
