using System.Security.Cryptography;
using System.Text;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Models;

namespace CRM.Application.Security
{
  /// <summary>
  ///   Provides methods for hashing and verifying passwords
  /// </summary>
  public static class HesherService
  {
    /// <summary>
    ///   Hashes a password using SHA256
    /// </summary>
    /// <returns>The hashed password</returns>
    public static string GetPasswordHash(User user)
    {
      string date = user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      return GetHash(user.Password, saltArray);
    }

    /// <summary>
    ///   Hashes a password using SHA256
    /// </summary>
    /// <returns>The hashed password</returns>
    public static string GetPasswordHash(User user, string password)
    {
      string date = user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      return GetHash(password, saltArray);
    }

    /// <summary>
    ///   Generates a random password of the specified length
    /// </summary>
    /// <param name="length">The length of the password to be generated</param>
    /// <returns>A random password as a string</returns>
    /// <exception cref="CustomException"></exception>
    public static string GetRandomPassword(int length)
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

    /// <summary>
    ///   Checks if the provided password matches the stored password for the given user
    /// </summary>
    /// <param name="user">The user whose password needs to be checked</param>
    /// <param name="requestPassword">The password provided in the request</param>
    /// <returns>True if the provided password matches the stored password, otherwise false</returns>
    public static bool PasswordСheck(User user, string requestPassword)
    {
      string date = user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = GetHash(requestPassword, saltArray);
      return hash == user.Password;
    }

    /// <summary>
    ///   Generates a hash for the given password using the specified salt
    /// </summary>
    /// <param name="password">The password to be hashed</param>
    /// <param name="salt">The salt to be used in the hashing process</param>
    /// <returns>The hashed password as a Base64-encoded string</returns>
    /// <remarks>
    ///   This method uses the PBKDF2 (Password-Based Key Derivation Function 2) algorithm with SHA256 to generate the hash.
    ///   The number of iterations is set to 1000.
    /// </remarks>
    private static string GetHash(string password, byte[] salt)
    {
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256);
      byte[] hashArray = pbkdf2.GetBytes(20);
      string result = Convert.ToBase64String(hashArray);
      pbkdf2.Dispose();
      return result;
    }

    /// <summary>
    ///   Generates a cryptographically secure random number within a specified range
    /// </summary>
    /// <param name="min">The inclusive lower bound of the random number returned</param>
    /// <param name="max">The exclusive upper bound of the random number returned. Must be greater than min</param>
    /// <returns>
    ///   A cryptographically secure random number within the specified range. 
    ///   Returns 0 if the min value is greater than or equal to the max value.
    /// </returns>
    private static int GetRandomNumber(int min, int max)
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