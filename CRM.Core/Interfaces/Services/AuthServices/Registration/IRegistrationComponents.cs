using CRM.Core.Exceptions.Custom;

namespace CRM.Core.Interfaces.Services.AuthServices.Registration
{
    /// <summary>
    ///   Provides utility methods for user registration components.
    /// </summary>
    public interface IRegistrationComponents
    {
        /// <summary>
        ///   Ensures that the specified role is not "Admin".
        /// </summary>
        /// <param name="post">The role to check.</param>
        /// <exception cref="CustomException">Thrown if the role is "Admin".</exception>
        void EnsureNonAdminRole(string post);

        /// <summary>
        ///   Decodes a Base64 encoded string.
        /// </summary>
        /// <param name="code">The Base64 encoded string to decode.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="FormatException">Thrown if the input is not a valid Base64 encoded string.</exception>
        string GetStringFromBase64(string code);
    }
}