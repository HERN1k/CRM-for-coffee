using System.Text;

using CRM.Application.RequestDataMapper;
using CRM.Application.RequestValidation;
using CRM.Application.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class RegisterService(
      IOptions<HttpSettings> httpSettings,
      IOptions<EmailConfirmRegisterSettings> emailConfirmRegisterSettings,
      IRepository repository,
      IEmailSender emailSender
    ) : IRegisterService
  {
    private readonly HttpSettings _httpSettings = httpSettings.Value;
    private readonly EmailConfirmRegisterSettings _emailConfirmRegisterSettings = emailConfirmRegisterSettings.Value;
    private readonly IRepository _repository = repository;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<IActionResult> RegisterNewWorkerAsync(RegisterRequest request)
    {
      if (request.Post == "Admin")
        throw new CustomException(ErrorTypes.ValidationError, "You cannot register a user with administrator rights");

      User user = RequestMapper.MapToModel(request);

      await RegistrationСheck(user);

      user.Password = HesherService.GetPasswordHash(user);

      await _emailSender.SendEmailConfirmRegisterAsync(user, _httpSettings, _emailConfirmRegisterSettings);

      var newUser = await SaveNewWorker(user);

      await _emailSender.NotifyAdminsOnManagerRegistration(newUser, GetAdminList());

      return new OkResult();
    }

    public async Task<IActionResult> ConfirmRegisterAsync(string code)
    {
      User user = new User { Email = GetStringFromBase64(code) };

      RequestValidator.ValidateEmail(user.Email);

      await SetTrueIsConfirmed(user);

      return new OkResult();
    }

    private async Task RegistrationСheck(User user)
    {
      bool isRegistered = await _repository
        .AnyAsync<EntityUser>((e) => e.Email == user.Email);
      if (isRegistered)
        throw new CustomException(ErrorTypes.BadRequest, "The user has already been registered");
    }
    private async Task<EntityUser> SaveNewWorker(User user)
    {
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
      await _repository.AddAsync<EntityUser>(newUser);
      return newUser;
    }
    private List<EntityUser> GetAdminList() =>
      [.. _repository
        .GetQueryable<EntityUser>()
        .AsNoTracking()
        .Where(e => e.Post == "Admin")];
    private string GetStringFromBase64(string code)
    {
      byte[] bytes = Convert.FromBase64String(code);
      return Encoding.UTF8.GetString(bytes);
    }
    private async Task SetTrueIsConfirmed(User user)
    {
      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == user.Email)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      entityUser.IsConfirmed = true;

      await _repository.UpdateAsync(entityUser);
    }
  }
}