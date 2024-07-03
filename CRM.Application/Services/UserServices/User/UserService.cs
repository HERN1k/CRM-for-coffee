using CRM.Core.Entities;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Services.UserServices;

namespace CRM.Application.Services.UserServices.User
{
  public class UserService(
      IBaseRepository repository
    ) : IUserService
  {
    private readonly IBaseRepository _repository = repository;

    public IQueryable<EntityUser> GetUsers() =>
      _repository.GetQueryable<EntityUser>();
  }
}