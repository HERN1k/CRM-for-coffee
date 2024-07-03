using CRM.Core.Entities;

namespace CRM.Core.Interfaces.Services.UserServices
{
    public interface IUserService
    {
        IQueryable<EntityUser> GetUsers();
    }
}