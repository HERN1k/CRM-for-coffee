using CRM.Core.Entities;

namespace CRM.Core.Interfaces.UserServices
{
  public interface IUserService
  {
    IQueryable<EntityUser> GetUsers();
  }
}