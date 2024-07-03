using CRM.Core.Contracts.RestDto;
using CRM.Core.Exceptions.Custom;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.AuthServices.Registration
{
    /// <summary>
    ///   Provides methods for registering new users and confirming email addresses.
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        ///   Registers a new worker asynchronously.
        /// </summary>
        /// <param name="request">The registration request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if the user role is admin or if the user is already registered.</exception>
        Task<IActionResult> RegistrationNewWorkerAsync(RegistrationRequest request);

        /// <summary>
        ///   Confirms the email address asynchronously.
        /// </summary>
        /// <param name="code">The base64 encoded email confirmation code.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if the email is not valid or already confirmed.</exception>
        Task<IActionResult> ConfirmEmailAsync(string code);
    }
}