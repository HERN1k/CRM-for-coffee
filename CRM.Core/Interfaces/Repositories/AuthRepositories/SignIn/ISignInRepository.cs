using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.AuthRepositories.SignIn
{
  /// <summary>
  ///   Provides methods for managing sign-in processes.
  /// </summary>
  public interface ISignInRepository
  {
    /// <summary>
    ///   Finds a worker by email.
    /// </summary>
    /// <param name="email">The email of the worker to find.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the found <see cref="User"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the email is null or empty.</exception>
    /// <exception cref="CustomException">Thrown if the user is not registered.</exception>
    Task<User> FindWorker(string email);

    /// <summary>
    ///   Saves or updates a refresh token for the specified user ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="token">The refresh token to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the token is null or empty.</exception>
    /// <exception cref="CustomException">Thrown if there is a server error.</exception>
    Task SaveToken(Guid id, string token);
  }
}