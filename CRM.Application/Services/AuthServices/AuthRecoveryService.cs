using System.Text;

using CRM.Application.RegEx;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.PasswordHesher;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Models;

namespace CRM.Application.Services.AuthServices
{
  public class AuthRecoveryService : IAuthRecoveryService
  {
    private readonly IRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IHesherService _hashPassword;
    private User? _user { get; set; }

    public AuthRecoveryService(
        IRepository repository,
        IEmailService emailService,
        IHesherService hashPassword
      )
    {
      _repository = repository;
      _emailService = emailService;
      _hashPassword = hashPassword;
    }

    public async Task ValidationDataRecoveryPass(RecoveryPasswordRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool phoneNumber = RegExHelper.ChackString(request.phoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        throw new CustomException(ErrorTypes.ValidationError, "Phone number is incorrect or null");

      bool post = RegExHelper.ChackString(request.post, RegExPatterns.Post);
      if (!post)
        throw new CustomException(ErrorTypes.ValidationError, "Post is incorrect or null");

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == request.email)
        ?? throw new CustomException(ErrorTypes.BadRequest, "The user is not registered");

      _user = new User
      {
        Id = user.Id,
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
    }

    public void СomparisonRecoveryPassData(RecoveryPasswordRequest request)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      if (_user.PhoneNumber != request.phoneNumber)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
      if (_user.Post != request.post)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
    }

    public string GetNewPassword(int length)
    {
      string newPassword;
      bool isCorrect;
      int iterations = 0;
      do
      {
        newPassword = _hashPassword.GetRandomPassword(length);
        isCorrect = RegExHelper.ChackString(newPassword, RegExPatterns.Password);
        iterations++;
      } while (!(isCorrect || iterations >= 10));

      if (string.IsNullOrEmpty(newPassword) || iterations >= 10)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      return newPassword;
    }
    public async Task SendRecoveryPassEmail(string password)
    {
      try
      {
        if (_user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        await _emailService.SendEmailRecoveryPassword(_user.FirstName, _user.Email, password);
      }
      catch (CustomException ex)
      {
        if (ex.ErrorType == ErrorTypes.MailKitException)
          throw new CustomException(ErrorTypes.MailKitException, "The email was not sent");
        throw;
      }
    }

    public async Task SaveNewPassword(string password)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string date = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(password, saltArray);

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Id == _user.Id)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      user.Password = hash;

      await _repository.UpdateAsync<EntityUser>(user);
    }

    public async Task RemoveRefreshToken()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == _user.Id);
    }
  }
}