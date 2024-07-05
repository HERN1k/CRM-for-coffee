﻿using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.AuthRepositories.AuthRecovery
{
  /// <summary>
  ///   Provides methods for handling authentication recovery processes.
  /// </summary>
  public interface IAuthRecoveryRepository
  {
    /// <summary>
    ///   Finds a worker by email.
    /// </summary>
    /// <param name="email">The email of the worker to find.</param>
    /// <param name="type">The error type to throw if the worker is not found.</param>
    /// <param name="errorMessage">The error message to throw if the worker is not found.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the found <see cref="User"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the email or errorMessage is null or empty.</exception>
    /// <exception cref="CustomException">Thrown if the worker is not found.</exception>
    Task<User> FindWorker(string email, ErrorTypes type, string errorMassage);

    /// <summary>
    ///   Saves a new password hash for the user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="hash">The new password hash.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the hash is null or empty.</exception>
    /// <exception cref="CustomException">Thrown if the user is not found.</exception>
    Task SaveNewPassword(Guid id, string hash);

    /// <summary>
    ///   Removes the refresh token for the specified user ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveRefreshToken(Guid id);
  }
}