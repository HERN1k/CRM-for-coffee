using CRM.Core.Contracts.GraphQlDto;

namespace CRM.Core.Interfaces.OrderServices
{
  public interface IOrderService
  {
    void ValidateOrderData(CreateOrderRequest request);
    Task CreateOrder(CreateOrderRequest request);
    Task SaveOrder();
  }
}