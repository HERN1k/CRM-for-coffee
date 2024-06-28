using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Responses;

namespace CRM.Core.Interfaces.OrderServices
{
  public interface IOrderService
  {
    Task<OperationResult> AddOrderAsync(CreateOrderRequest input);
    Task<IEnumerable<EntityOrder>> GetEmployeeOrdersForDayAsync();
    IQueryable<EntityOrder> GetOrders();
  }
}