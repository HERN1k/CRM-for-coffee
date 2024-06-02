namespace CRM.Core.Interfaces.AuthServices
{
  public interface ISignOutService
  {
    string TokenDecryption(string token);
    Task RemoveToken(string email, string refreshToken);
  }
}