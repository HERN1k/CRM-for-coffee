using CRM.Core.Entities;
using CRM.Core.Interfaces.Services.UserServices;
using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
    public partial class Queries
  {
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    [Authorize(Policy = "ManagerOrUpper")]
    public IQueryable<EntityUser> GetUsers(
      [Service] IUserService userService
      ) => userService.GetUsers();
  }
}