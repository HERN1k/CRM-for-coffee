using CRM.Core.Contracts.RestDto;
using CRM.Core.Exceptions.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.AuthServices.AuthRecovery
{
    /// <summary>
    ///   Provides services for handling password recovery processes.
    /// </summary>
    public interface IAuthRecoveryService
    {
        /// <summary>
        ///   Handles the process of recovering a password asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="request">The password recovery request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if there is an issue with the password recovery process.</exception>
        Task<IActionResult> RecoveryPasswordAsync(HttpContext httpContext, RecoveryPasswordRequest request);
    }
}