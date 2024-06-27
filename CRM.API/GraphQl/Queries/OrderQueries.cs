using CRM.Core.Entities;
using CRM.Core.Interfaces.OrderServices;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  public partial class Queries
  {
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [Authorize(Policy = "AdminOrLower")]
    public async Task<IEnumerable<EntityOrder>> GetEmployeeOrdersForDay(
        [Service] IOrderService orderService
      ) => await orderService.GetEmployeeOrdersForDayAsync();

    [UseProjection]
    [UseSorting]
    [UseFiltering]
    [Authorize(Policy = "ManagerOrUpper")]
    public IQueryable<EntityOrder> GetOrders(
        [Service] IOrderService orderService
      ) => orderService.GetOrdersAsync();
  }
}