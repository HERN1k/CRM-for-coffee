using CRM.Core.Contracts.RestDto;
using CRM.Core.Exceptions.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.AuthServices.SignIn
{
    /// <summary>
    ///   Provides services for handling user sign-in processes.
    /// </summary>
    public interface ISignInService
    {
        /// <summary>
        ///   Handles the sign-in process asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="request">The sign-in request containing the user credentials.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        /// <exception cref="CustomException">Thrown if the request is invalid or if the password is incorrect.</exception>
        Task<IActionResult> SignInAsync(HttpContext httpContext, SignInRequest request);
    }
}
