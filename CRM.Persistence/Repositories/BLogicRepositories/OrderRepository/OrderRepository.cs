using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.OrderRepository;
using CRM.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Repositories.BLogicRepositories.OrderRepository
{
  public class OrderRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IOrderRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<User> FindWorkerByEmail(string email, ErrorTypes type, string message)
    {
      var entity = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, message);

      var result = _mapper.Map<User>(entity);

      return result;
    }

    public async Task<TModel> FindEntityById<TModel, TEntity>(Guid id, ErrorTypes type, string message) where TModel : class, IEntityWithId where TEntity : class, IEntityWithId
    {
      var entity = await _repository
        .FindSingleAsync<TEntity>(e => e.Id == id)
          ?? throw new CustomException(type, message);

      var result = _mapper.Map<TModel>(entity);

      return result;
    }

    public List<Order> GetOrdersByDescendingWorkerPerDay(User user)
    {
      var entities = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Include(e => e.Products.AsQueryable())
          .ThenInclude(e => e.AddOns)
        .OrderByDescending(e => e.OrderСreationDate);

      var result = new List<Order>();

      foreach (var entity in entities)
      {
        Order temp = _mapper.Map<Order>(entity);
        result.Add(temp);
      }

      return result;
    }

    public int GetNumberOrders(User user)
    {
      return _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Count();
    }

    public IQueryable<Order> GetQueryableOrders()
    {
      var entities = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns);

      var result = new List<Order>();

      foreach (var entity in entities)
      {
        Order temp = _mapper.Map<Order>(entity);

        result.Add(temp);
      }

      return result.AsQueryable();
    }

    public async Task AddOrderAsync(Order order)
    {
      var newOrder = _mapper.Map<EntityOrder>(order);

      await _repository.AddAsync<EntityOrder>(newOrder);
    }
  }
}