using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.AuthRepositories.Registration;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Repositories.AuthRepositories.Registration
{
  public class RegistrationRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IRegistrationRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task RegistrationСheck(string email)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      bool isRegistered = await _repository
        .AnyAsync<EntityUser>((e) => e.Email == email);

      if (isRegistered)
        throw new CustomException(ErrorTypes.BadRequest, "The user has already been registered");
    }

    public async Task<User> SaveNewWorker(User user)
    {
      ArgumentNullException.ThrowIfNull(user);

      var newUser = _mapper.Map<EntityUser>(user);

      await _repository.AddAsync(newUser);

      User result = _mapper.Map<User>(newUser);

      return result;
    }

    public List<User> GetAdminList()
    {
      var entities = _repository.GetQueryable<EntityUser>()
        .AsNoTracking()
        .Where(e => e.Post == "Admin")
        .ToList();

      var result = new List<User>();

      foreach (var entity in entities)
      {
        User temp = _mapper.Map<User>(entity);
        result.Add(temp);
      }

      return result;
    }

    public async Task CheckingMailConfirmation(string email)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(ErrorTypes.BadRequest, "User with this email not found");

      if (entityUser.IsConfirmed)
        throw new CustomException(ErrorTypes.BadRequest, "Email address has already been confirmed");
    }

    public async Task SetTrueIsConfirmed(string email)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(ErrorTypes.BadRequest, "User with this email not found");

      entityUser.IsConfirmed = true;

      await _repository.UpdateAsync(entityUser);
    }
  }
}