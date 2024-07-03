using CRM.Core.Exceptions.Custom;

namespace CRM.Core.Interfaces.Repositories.SignOut
{
    /// <summary>
    ///   Provides methods for handling user sign-out processes.
    /// </summary>
    public interface ISignOutRepository
    {
        /// <summary>
        ///   Removes the refresh token for the specified user ID.
        /// </summary>
        /// <param name="id">The ID of the user whose token is to be removed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CustomException">Thrown if the ID is not a valid GUID.</exception>
        Task RemoveToken(string id);
    }
}