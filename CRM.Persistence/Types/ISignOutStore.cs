namespace CRM.Data.Types
{
  public interface ISignOutStore
  {
    Task RemoveToken(string email, string refreshToken);
  }
}
