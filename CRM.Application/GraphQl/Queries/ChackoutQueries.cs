using CRM.Core.Entities;
using CRM.Data;

using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;

namespace CRM.Application.GraphQl.Queries
{
  [Authorize(Policy = "AdminOrLower")]
  public class ChackoutQueries
  {
    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<User> GetUsers([Service] AppDBContext _context)
    {
      var result = _context.Users;

      return result;
    }
  }
}