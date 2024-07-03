using CRM.Core.Contracts.RestDto;
using CRM.Core.Exceptions.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.AuthServices.AuthSundry
{
    /// <summary>
    ///   Provides services for handling various authentication processes.
    /// </summary>
    public interface IAuthSundryService
    {
        /// <summary>
        ///   Handles the process of changing the token asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="request">The change token request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if there is an issue with the token or email validation.</exception>
        Task<IActionResult> ChangeTokenAsync(HttpContext httpContext, ChangeTokenRequest request);

        /// <summary>
        ///   Handles the process of updating the password asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="request">The update password request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if there is an issue with the password validation or update process.</exception>
        Task<IActionResult> UpdatePasswordAsync(HttpContext httpContext, UpdatePasswordRequest request);
    }
}