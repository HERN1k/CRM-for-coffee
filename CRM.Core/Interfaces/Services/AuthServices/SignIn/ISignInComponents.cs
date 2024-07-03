using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.Services.AuthServices.SignIn
{
    /// <summary>
    ///   Provides utility methods for sign-in components.
    /// </summary>
    public interface ISignInComponents
    {
        /// <summary>
        ///   Checks if the provided password is correct.
        /// </summary>
        /// <param name="user">The user whose password is being checked.</param>
        /// <param name="request">The sign-in request containing the password to check.</param>
        /// <exception cref="CustomException">Thrown if the password is incorrect.</exception>
        void ChackingCorrectPassword(User user, SignInRequest request);

        /// <summary>
        ///   Sets the cookie options based on the token type.
        /// </summary>
        /// <param name="typesTokens">The type of the token (Access or Refresh).</param>
        /// <returns>The configured <see cref="CookieOptions"/>.</returns>
        CookieOptions SetCookieOptions(TokenTypes typesTokens);

        /// <summary>
        ///   Sets the access and refresh tokens in the HTTP context cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        void SetCookie(HttpContext httpContext, string accessToken, string refreshToken);

        /// <summary>
        ///   Creates a dictionary of JWT tokens for the user.
        /// </summary>
        /// <param name="user">The user for whom the tokens are created.</param>
        /// <returns>A dictionary with the access and refresh tokens.</returns>
        Dictionary<TokenTypes, string> CreateJwtTokenDictionary(User user);

        /// <summary>
        ///   Creates a sign-in response for the user.
        /// </summary>
        /// <param name="user">The user who is signing in.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A <see cref="SignInResponse"/> object containing the user's information and the refresh token.</returns>
        SignInResponse CreateResponse(User user, string refreshToken);
    }
}