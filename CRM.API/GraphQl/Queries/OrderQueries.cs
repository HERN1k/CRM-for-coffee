using CRM.Core.Interfaces.Services.BLogicServices.OrderServices;
using CRM.Core.Models;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  public partial class Queries
  {
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [Authorize(Policy = "AdminOrLower")]
    public async Task<IEnumerable<Order>> GetEmployeeOrdersForDay(
        [Service] IOrderService orderService
      ) => await orderService.GetEmployeeOrdersForDayAsync();

    [UseProjection]
    [UseSorting]
    [UseFiltering]
    [Authorize(Policy = "ManagerOrUpper")]
    public IQueryable<Order> GetOrders(
        [Service] IOrderService orderService
      ) => orderService.GetOrders();
  }
}