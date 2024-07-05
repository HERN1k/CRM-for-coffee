using CRM.Core.Interfaces.Services.UserServices;
using CRM.Core.Models;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  public partial class Queries
  {
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    [Authorize(Policy = "ManagerOrUpper")]
    public IQueryable<User> GetUsers(
      [Service] IUserService userService
      ) => userService.GetUsers();
  }
}