namespace CRM.Core.Interfaces.PasswordHesher
{
  public interface IHesherService
  {
    string GetHash(string password, byte[] salt);
    string GetRandomPassword(int length);
    int GetRandomNumber(int min, int max);
  }
}
