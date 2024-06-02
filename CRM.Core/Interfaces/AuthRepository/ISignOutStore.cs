namespace CRM.Core.Interfaces.AuthRepository
{
  public interface ISignOutRepository
  {
    Task RemoveToken(string email, string refreshToken);
  }
}