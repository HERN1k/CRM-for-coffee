using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.AuthRepositories.AuthSundry;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Models;

namespace CRM.Data.Repositories.AuthRepositories.AuthSundry
{
  public class AuthSundryRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IAuthSundryRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<User> FindWorker(string email, ErrorTypes type, string errorMassage)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      if (string.IsNullOrEmpty(errorMassage))
        throw new ArgumentNullException(nameof(errorMassage));

      var entity = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, errorMassage);

      User result = _mapper.Map<User>(entity);

      return result;
    }

    public async Task CheckImmutableToken(Guid id, string refreshToken)
    {
      if (string.IsNullOrEmpty(refreshToken))
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      var token = await _repository
        .FindSingleAsync<EntityRefreshToken>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      if (token.RefreshTokenString != refreshToken)
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");
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

    public async Task RemoveRefreshToken(Guid id)
    {
      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == id);
    }
  }
}