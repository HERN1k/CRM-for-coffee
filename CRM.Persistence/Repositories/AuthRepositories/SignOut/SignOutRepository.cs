using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.AuthRepositories.SignOut;
using CRM.Core.Interfaces.Repositories.Base;

namespace CRM.Data.Repositories.AuthRepositories.SignOut
{
  public class SignOutRepository(
      IBaseRepository repository
    ) : ISignOutRepository
  {
    private readonly IBaseRepository _repository = repository;

    public async Task RemoveToken(string id)
    {
      bool isValid = Guid.TryParse(id, out Guid guid);
      if (!isValid)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == guid);
    }
  }
}