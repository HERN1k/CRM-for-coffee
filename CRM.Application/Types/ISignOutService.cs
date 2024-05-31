namespace CRM.Application.Types
{
  public interface ISignOutService
  {
    string TokenDecryption(string token);
    Task RemoveToken(string email, string refreshToken);
  }
}
