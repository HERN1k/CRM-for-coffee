namespace CRM.Data.Types
{
  public interface ISignOutStore
  {
    Task<bool> RemoveToken(string email, string refreshToken);
  }
}
