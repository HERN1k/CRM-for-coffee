using CRM.Core.Models;

namespace CRM.Core.Interfaces.Services.UserServices
{
  /// <summary>
  ///   Provides services for managing users.
  /// </summary>
  public interface IUserService
  {
    /// <summary>
    ///   Gets a queryable collection of users.
    /// </summary>
    /// <returns>A queryable collection of users.</returns>
    IQueryable<User> GetUsers();
  }
}