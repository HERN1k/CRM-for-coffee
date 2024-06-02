using System.Security.Cryptography;

using CRM.Core.Interfaces.PasswordHesher;

namespace CRM.Application.Security
{
  public class HesherService : IHesherService
  {
    public string GetHash(string password, byte[] salt)
    {
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256);
      byte[] hashArray = pbkdf2.GetBytes(20);
      string result = Convert.ToBase64String(hashArray);
      pbkdf2.Dispose();
      return result;
    }

    public int GetRandomNumber(int min, int max)
    {
      if (min >= max)
        return 0;
      uint range = (uint)(max - min);
      byte[] uint32Buffer = new byte[4];
      using var rng = RandomNumberGenerator.Create();
      while (true)
      {
        rng.GetBytes(uint32Buffer);
        uint randomUint = BitConverter.ToUInt32(uint32Buffer, 0);
        if (randomUint < uint.MaxValue - (uint.MaxValue % range))
          return (int)(randomUint % range) + min;
      }
    }
  }
}
