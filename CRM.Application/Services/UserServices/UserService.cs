using CRM.Core.Entities;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.UserServices;

namespace CRM.Application.Services.UserServices
{
  public class UserService(
      IRepository repository
    ) : IUserService
  {
    private readonly IRepository _repository = repository;

    public IQueryable<EntityUser> GetUsers() =>
      _repository.GetQueryable<EntityUser>();
  }
}