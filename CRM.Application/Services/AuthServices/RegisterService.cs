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
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class RegisterService(
      IOptions<HttpSettings> httpSettings,
      IOptions<EmailConfirmRegisterSettings> emailConfirmRegisterSettings,
      IRepository repository,
      IHesherService hashPassword,
      IEmailService emailService
    ) : IRegisterService
  {
    private readonly HttpSettings _httpSettings = httpSettings.Value;
    private readonly EmailConfirmRegisterSettings _emailConfirmRegisterSettings = emailConfirmRegisterSettings.Value;
    private readonly IRepository _repository = repository;
    private readonly IHesherService _hashPassword = hashPassword;
    private readonly IEmailService _emailService = emailService;
    private User? _user { get; set; }

    #region AddToModel
    public void AddToModel(RegisterRequest request)
    {
      _user = new User
      {
        FirstName = request.firstName,
        LastName = request.lastName,
        FatherName = request.fatherName,
        Email = request.email,
        Password = request.password,
        Post = request.post,
        Age = request.age,
        Gender = request.gender,
        PhoneNumber = request.phoneNumber,
        IsConfirmed = false,
        RegistrationDate = DateTime.UtcNow
      };
    }
    #endregion

    #region ValidationDataRegister
    public void ValidationDataRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      bool firstName = RegExHelper.ChackString(_user.FirstName, RegExPatterns.FirstName);
      if (!firstName)
        throw new CustomException(ErrorTypes.ValidationError, "First name is incorrect or null");

      bool lastName = RegExHelper.ChackString(_user.LastName, RegExPatterns.LastName);
      if (!lastName)
        throw new CustomException(ErrorTypes.ValidationError, "Last name is incorrect or null");

      bool fatherName = RegExHelper.ChackString(_user.FatherName, RegExPatterns.FatherName);
      if (!fatherName)
        throw new CustomException(ErrorTypes.ValidationError, "Father name is incorrect or null");

      bool password = RegExHelper.ChackString(_user.Password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");

      if (_user.Age < 16 || _user.Age > 65)
        throw new CustomException(ErrorTypes.ValidationError, "Age is incorrect or null");

      bool gender = RegExHelper.ChackString(_user.Gender, RegExPatterns.Gender);
      if (!gender)
        throw new CustomException(ErrorTypes.ValidationError, "Gender is incorrect or null");

      bool phoneNumber = RegExHelper.ChackString(_user.PhoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        throw new CustomException(ErrorTypes.ValidationError, "Phone number is incorrect or null");

      bool email = RegExHelper.ChackString(_user.Email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool post = RegExHelper.ChackString(_user.Post, RegExPatterns.Post);
      if (!post)
        throw new CustomException(ErrorTypes.ValidationError, "Post is incorrect or null");
    }
    #endregion

    #region UserIsRegister
    public async Task UserIsRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      bool isRegistered = await _repository.AnyAsync<EntityUser>((e) => e.Email == _user.Email);
      if (isRegistered)
        throw new CustomException(ErrorTypes.BadRequest, "The user has already been registered");
    }
    #endregion

    #region GetHash
    public void GetHash()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string date = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(_user.Password, saltArray);
      _user.Password = hash;
    }
    #endregion

    #region SaveNewUser
    public async Task SaveNewUser()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      var user = new EntityUser
      {
        FirstName = _user.FirstName,
        LastName = _user.LastName,
        FatherName = _user.FatherName,
        Email = _user.Email,
        Password = _user.Password,
        Post = _user.Post,
        Age = _user.Age,
        Gender = _user.Gender,
        PhoneNumber = _user.PhoneNumber,
        IsConfirmed = _user.IsConfirmed,
        RegistrationDate = _user.RegistrationDate
      };
      await _repository.AddAsync<EntityUser>(user);
    }
    #endregion

    #region SendEmailConfirmRegister
    public async Task SendEmailConfirmRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      byte[] bytes = Encoding.UTF8.GetBytes(_user.Email);
      string code = Convert.ToBase64String(bytes);
      string url = new StringBuilder()
        .Append(_httpSettings.Protocol)
        .Append("://")
        .Append(_httpSettings.Domaine)
        .Append("/Api/Auth/ConfirmRegister/")
        .Append(code)
        .ToString();

      string html = await _emailService
        .CompileHtmlStringAsync("ConfirmRegister", new ConfirmRegister
        {
          TitleCode = _emailConfirmRegisterSettings.TitleCode,
          Code = code,
          TitleLink = _emailConfirmRegisterSettings.TitleLink,
          Link = url
        });

      await _emailService.SendEmailAsync(_user.FirstName, _user.Email, html);
    }
    #endregion

    #region SendEmailToAdministrators
    public async Task SendEmailToAdministrators()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      if (_user.Post == "Worker")
        return;

      var admins = _repository
        .GetQueryable<EntityUser>()
        .AsNoTracking()
        .Where(e => e.Post == "Admin");

      var user = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == _user.Email)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      string post = user.Post.ToLower();
      string name = $"{user.FirstName} {_user.LastName} {_user.FatherName}";
      string workerId = user.Id.ToString();

      string html = await _emailService
        .CompileHtmlStringAsync("AddedNewManagerOrAdmin", new AddedNewManagerOrAdmin
        {
          Post = post,
          Name = name,
          WorkerId = workerId
        });

      foreach (var admin in admins)
      {
        await _emailService.SendEmailAsync(admin.FirstName, admin.Email, html);
      }
    }
    #endregion

    #region FromBase64ToString
    public void FromBase64ToString(string input)
    {
      if (string.IsNullOrEmpty(input))
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      byte[] bytes = Convert.FromBase64String(input);
      string result = Encoding.UTF8.GetString(bytes);
      _user = new User { Email = result };
    }
    #endregion

    #region ValidationEmail
    public void ValidationEmail()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      bool isSuccess = RegExHelper.ChackString(_user.Email, RegExPatterns.Email);
      if (!isSuccess)
        throw new CustomException(ErrorTypes.BadRequest, "Code is invalid");
    }
    #endregion

    #region ConfirmRegister
    public async Task ConfirmRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == _user.Email)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      user.IsConfirmed = true;

      await _repository.UpdateAsync(user);
    }
    #endregion
  }
}