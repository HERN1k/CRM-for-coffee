namespace CRM.Core.Interfaces.Infrastructure.Email
{
    /// <summary>
    ///   Provides methods for compiling HTML strings and sending emails using Razor templates and MailKit.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        ///   Compiles an HTML string using a Razor template and the specified model.
        /// </summary>
        /// <typeparam name="T">The type of the model.</typeparam>
        /// <param name="key">The key identifying the Razor template.</param>
        /// <param name="model">The model to use for the template.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the compiled HTML string.</returns>
        Task<string> CompileHtmlStringAsync<T>(string key, T model) where T : class;

        /// <summary>
        ///   Sends an email asynchronously with the specified recipient's name, email address, and HTML content.
        /// </summary>
        /// <param name="name">The name of the recipient.</param>
        /// <param name="email">The email address of the recipient.</param>
        /// <param name="html">The HTML content of the email body.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailAsync(string name, string email, string html);
    }
}