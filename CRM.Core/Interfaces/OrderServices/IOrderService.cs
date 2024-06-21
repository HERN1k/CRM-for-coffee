using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;

namespace CRM.Core.Interfaces.OrderServices
{
  public interface IOrderService
  {
    void ValidateOrderData(CreateOrderRequest request);
    Task CreateOrder(CreateOrderRequest request);
    Task SaveOrder();
    Task<IEnumerable<EntityOrder>> GetEmployeeOrdersForDay();
    IQueryable<EntityOrder> GetOrders();
  }
}