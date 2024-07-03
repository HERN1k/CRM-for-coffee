using CRM.Core.Entities;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.Registration
{
    /// <summary>
    ///   Provides methods for managing user registrations.
    /// </summary>
    public interface IRegistrationRepository
    {
        /// <summary>
        ///   Checks if the email is already registered.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the email is null or empty.</exception>
        /// <exception cref="CustomException">Thrown if the email is already registered.</exception>
        Task RegistrationСheck(string email);

        /// <summary>
        ///   Saves a new worker to the database.
        /// </summary>
        /// <param name="user">The user to save.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the saved <see cref="EntityUser"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the user is null.</exception>
        Task<EntityUser> SaveNewWorker(User user);

        /// <summary>
        ///   Gets a list of all administrators.
        /// </summary>
        /// <returns>A list of <see cref="EntityUser"/> who are administrators.</returns>
        List<EntityUser> GetAdminList();

        /// <summary>
        ///   Checks if the user's email is already confirmed.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the email is null or empty.</exception>
        /// <exception cref="CustomException">Thrown if the email is not found or already confirmed.</exception>
        Task CheckingMailConfirmation(string email);

        /// <summary>
        ///   Sets the IsConfirmed property to true for the user with the specified email.
        /// </summary>
        /// <param name="email">The email of the user to confirm.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the email is null or empty.</exception>
        /// <exception cref="CustomException">Thrown if the email is not found.</exception>
        Task SetTrueIsConfirmed(string email);
    }
}