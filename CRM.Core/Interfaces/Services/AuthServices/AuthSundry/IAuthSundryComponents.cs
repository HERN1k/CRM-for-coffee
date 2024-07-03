using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.Services.AuthServices.AuthSundry
{
    /// <summary>
    ///   Provides auxiliary methods for authentication management.
    /// </summary>
    public interface IAuthSundryComponents
    {
        /// <summary>
        ///   Retrieves the refresh token from the HTTP context cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The refresh token.</returns>
        /// <exception cref="CustomException">Thrown if the refresh token is not found.</exception>
        string GetRefreshToken(HttpContext httpContext);

        /// <summary>
        ///   Sets the cookie options based on the token type.
        /// </summary>
        /// <param name="typesTokens">The type of the token (Access or Refresh).</param>
        /// <returns>The configured <see cref="CookieOptions"/>.</returns>
        CookieOptions SetCookieOptions(TokenTypes typesTokens);

        /// <summary>
        ///   Sets the access token in the HTTP context cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="accessToken">The access token.</param>
        /// <exception cref="ArgumentNullException">Thrown if the access token is null or empty.</exception>
        void SetAccessTokenCookie(HttpContext httpContext, string accessToken);

        /// <summary>
        ///   Sets cookies to unauthorized state.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        void SetUnauthorizedCookies(HttpContext httpContext);

        /// <summary>
        ///   Checks if the new password is different from the old password.
        /// </summary>
        /// <param name="request">The update password request.</param>
        /// <exception cref="CustomException">Thrown if the new password is the same as the old password.</exception>
        void CheckPasswordDifference(UpdatePasswordRequest request);

        /// <summary>
        ///   Checks if the provided password is correct.
        /// </summary>
        /// <param name="user">The user whose password is being checked.</param>
        /// <param name="requestPassword">The password to check.</param>
        /// <exception cref="ArgumentNullException">Thrown if the password is null or empty.</exception>
        /// <exception cref="CustomException">Thrown if the password is incorrect.</exception>
        void PasswordСheck(User user, string requestPassword);
    }
}