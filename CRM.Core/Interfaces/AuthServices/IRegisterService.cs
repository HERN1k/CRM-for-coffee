using CRM.Core.Contracts.RestDto;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IRegisterService
  {
    void AddToModel(RegisterRequest request);
    void ValidationDataRegister();
    Task UserIsRegister();
    void GetHash();
    Task SaveNewUser();
    Task SendEmailConfirmRegister();
    Task SendEmailToAdministrators();
    void FromBase64ToString(string input);
    void ValidationEmail();
    Task ConfirmRegister();
  }
}
