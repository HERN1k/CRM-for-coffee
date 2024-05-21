namespace CRM.Application.Types
{
  public interface ISignOutService
  {
    string TokenDecryption(string token);
    Task<bool> RemoveToken(string email, string refreshToken);
  }
}
