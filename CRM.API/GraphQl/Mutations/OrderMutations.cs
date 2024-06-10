using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Interfaces.OrderServices;
using CRM.Core.Responses;

namespace CRM.API.GraphQl.Mutations
{
  //[Authorize(Policy = "AdminOrLower")]
  public partial class Mutations
  {
    public async Task<OperationResult> AddOrder(
        [Service] IOrderService orderService,
        CreateOrderRequest input
      )
    {
      orderService.ValidateOrderData(input);

      await orderService.CreateOrder(input);

      await orderService.SaveOrder();

      return new OperationResult(true);
    }
  }
}