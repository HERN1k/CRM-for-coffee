namespace CRM.Core.Interfaces.PasswordHesher
{
  public interface IHesherService
  {
    string GetHash(string password, byte[] salt);
    int GetRandomNumber(int min, int max);
  }
}
