using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.AuthRecovery;
using CRM.Core.Interfaces.Repositories.Base;

namespace CRM.Data.Repositories.AuthRecovery
{
  public class AuthRecoveryRepository(
      IBaseRepository repository
    ) : IAuthRecoveryRepository
  {
    private readonly IBaseRepository _repository = repository;

    public async Task<EntityUser> FindWorker(string email, ErrorTypes type, string errorMassage)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      if (string.IsNullOrEmpty(errorMassage))
        throw new ArgumentNullException(nameof(errorMassage));

      var result = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, errorMassage);

      return result;
    }

    public async Task SaveNewPassword(Guid id, string hash)
    {
      if (string.IsNullOrEmpty(hash))
        throw new ArgumentNullException(nameof(hash));

      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      entityUser.Password = hash;

      await _repository.UpdateAsync(entityUser);
    }

    public async Task RemoveRefreshToken(Guid id) =>
      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == id);
  }
}