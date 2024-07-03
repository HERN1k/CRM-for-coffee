using CRM.Core.Contracts.RestDto;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.Services.AuthServices.AuthRecovery
{
    /// <summary>
    ///   Provides auxiliary methods for authentication recovery processes.
    /// </summary>
    public interface IAuthRecoveryComponents
    {
        /// <summary>
        ///   Compares the recovery password request data with the user's data.
        /// </summary>
        /// <param name="user">The user whose data is being compared.</param>
        /// <param name="request">The recovery password request containing the data to compare.</param>
        /// <exception cref="CustomException">Thrown if any data doesn't match.</exception>
        public
        void СomparisonRecoveryPassData(User user, RecoveryPasswordRequest request);

        /// <summary>
        ///   Generates a new random password of the specified length.
        /// </summary>
        /// <param name="length">The length of the new password.</param>
        /// <returns>The generated new password.</returns>
        /// <exception cref="CustomException">Thrown if the password generation fails after multiple attempts.</exception>
        string GetNewPassword(int length);

        /// <summary>
        ///   Sets cookies to unauthorized state.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        void SetUnauthorizedCookies(HttpContext httpContext);
    }
}