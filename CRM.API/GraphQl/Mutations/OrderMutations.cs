using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Interfaces.Services.BLogicServices.OrderServices;
using CRM.Core.Responses;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Mutations
{
  public partial class Mutations
  {
    [Authorize(Policy = "AdminOrLower")]
    public async Task<OperationResult> AddOrder(
        [Service] IOrderService orderService,
        CreateOrderRequest input
      ) => await orderService.AddOrderAsync(input);
  }
}