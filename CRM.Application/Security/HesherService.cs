using System.Security.Cryptography;
using System.Text;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
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

    public string GetRandomPassword(int length)
    {
      if (length < 8)
        throw new CustomException(ErrorTypes.InvalidOperationException, "The minimum line length must be 8 characters");

      const string validUpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      const string validLowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
      const string validNumbers = "0123456789";
      const string validSigns = "@$!%*?&";
      int otherСharsLength = (int)Math.Ceiling((float)(length / 4));

      var stringBuilder = new StringBuilder(length);

      for (int i = 0; i < length; i++)
      {
        stringBuilder.Insert(i, validUpperCaseChars[GetRandomNumber(0, validUpperCaseChars.Length - 1)]);
      }
      for (int i = 0; i < otherСharsLength; i++)
      {
        stringBuilder.Insert(
          GetRandomNumber(0, length - 1),
          validLowerCaseChars[GetRandomNumber(0, validLowerCaseChars.Length - 1)]
        );
      }
      for (int i = 0; i < otherСharsLength; i++)
      {
        stringBuilder.Insert(
          GetRandomNumber(0, length - 1),
          validNumbers[GetRandomNumber(0, validNumbers.Length - 1)]
        );
      }
      for (int i = 0; i < otherСharsLength; i++)
      {
        stringBuilder.Insert(
          GetRandomNumber(0, length - 1),
          validSigns[GetRandomNumber(0, validSigns.Length - 1)]
        );
      }

      return stringBuilder.ToString();
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
