using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.AuthServices.SignOut
{
    /// <summary>
    ///   Provides services for handling user sign-out processes.
    /// </summary>
    public interface ISignOutService
    {
        /// <summary>
        ///   Handles the logout process asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        public
        Task<IActionResult> LogoutAsync(HttpContext httpContext);
    }
}