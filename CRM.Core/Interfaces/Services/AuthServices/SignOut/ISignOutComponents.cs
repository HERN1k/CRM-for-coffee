using CRM.Core.Exceptions.Custom;
using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.Services.AuthServices.SignOut
{
    /// <summary>
    ///   Provides utility methods for handling sign-out processes.
    /// </summary>
    public interface ISignOutComponents
    {
        /// <summary>
        ///   Retrieves the access token from the HTTP context cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The access token.</returns>
        /// <exception cref="CustomException">Thrown if the access token is not found.</exception>
        string GetAccessToken(HttpContext httpContext);

        /// <summary>
        ///   Clears the access and refresh tokens from the HTTP context cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public void SetCookie(HttpContext httpContext);
    }
}