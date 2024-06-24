using CRM.Core.Entities;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.UserServices;

namespace CRM.Application.Services.UserServices
{
  public class UserService : IUserService
  {
    private readonly IRepository _repository;

    public UserService(
        IRepository repository
      )
    {
      _repository = repository;
    }

    #region GetUsers
    public IQueryable<EntityUser> GetUsers() =>
      _repository.GetQueryable<EntityUser>();
    #endregion
  }
}