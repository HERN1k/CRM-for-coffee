﻿using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.AuthRepositories.SignIn;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Models;

namespace CRM.Data.Repositories.AuthRepositories.SignIn
{
  public class SignInRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : ISignInRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<User> FindWorker(string email)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      var entity = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(ErrorTypes.BadRequest, "The user is not registered");

      User result = _mapper.Map<User>(entity);

      return result;
    }

    public async Task SaveToken(Guid id, string token)
    {
      if (string.IsNullOrEmpty(token))
        throw new ArgumentNullException(nameof(token));

      bool tokenSaved = await _repository
        .AnyAsync<EntityRefreshToken>(e => e.Id == id);

      if (tokenSaved)
      {
        var updateToken = await _repository.FindSingleAsync<EntityRefreshToken>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

        updateToken.RefreshTokenString = token;

        await _repository.UpdateAsync(updateToken);
        return;
      }

      var seveToken = new EntityRefreshToken { Id = id, RefreshTokenString = token };

      await _repository.AddAsync(seveToken);
    }
  }
}