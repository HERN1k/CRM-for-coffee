using CRM.Core.Models;

namespace CRM.Core.Interfaces.Tools.Security.PasswordHesher
{
    public interface IHesherService
    {
        string GetPasswordHash(User user);
        string GetHash(string password, byte[] salt);
        string GetRandomPassword(int length);
        int GetRandomNumber(int min, int max);
    }
}