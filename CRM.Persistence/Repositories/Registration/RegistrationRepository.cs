using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.Registration;
using CRM.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Repositories.Registration
{
  public class RegistrationRepository(
      IBaseRepository repository
    ) : IRegistrationRepository
  {
    private readonly IBaseRepository _repository = repository;

    public async Task RegistrationСheck(string email)
    {
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException(nameof(email));

      bool isRegistered = await _repository
        .AnyAsync<EntityUser>((e) => e.Email == email);

      if (isRegistered)
        throw new CustomException(ErrorTypes.BadRequest, "The user has already been registered");
    }

    public async Task<EntityUser> SaveNewWorker(User user)
    {
      if (user == null)
        throw new ArgumentNullException(nameof(user));

      var newUser = new EntityUser
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        FatherName = user.FatherName,
        Email = user.Email,
        Password = user.Password,
        Post = user.Post,
        Age = user.Age,
        Gender = user.Gender,
        PhoneNumber = user.PhoneNumber,
        IsConfirmed = user.IsConfirmed,
        RegistrationDate = user.RegistrationDate
      };

      await _repository.AddAsync(newUser);

      return newUser;
    }

    public List<EntityUser> GetAdminList() =>
      _repository.GetQueryable<EntityUser>()
        .AsNoTracking()
        .Where(e => e.Post == "Admin")
        .ToList();

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