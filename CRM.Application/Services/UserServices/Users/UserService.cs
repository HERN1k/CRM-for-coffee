using CRM.Core.Entities;
using CRM.Core.Interfaces.Repositories.UserRepositories;
using CRM.Core.Interfaces.Services.UserServices;
using CRM.Core.Models;

namespace CRM.Application.Services.UserServices.Users
{
  public class UserService(
      IUserRepository repository
    ) : IUserService
  {
    private readonly IUserRepository _repository = repository;

    public IQueryable<User> GetUsers() =>
      _repository.GetQueryable<User, EntityUser>();
  }
}