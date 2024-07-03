using System.Text;

using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Services.AuthServices.Registration;

namespace CRM.Application.Services.AuthServices.Registration
{
  public class RegistrationComponents : IRegistrationComponents
  {
    public void EnsureNonAdminRole(string post)
    {
      if (post == "Admin")
        throw new CustomException(ErrorTypes.ValidationError, "You cannot register a user with administrator rights");
    }

    public string GetStringFromBase64(string code)
    {
      byte[] bytes = Convert.FromBase64String(code);
      return Encoding.UTF8.GetString(bytes);
    }
  }
}