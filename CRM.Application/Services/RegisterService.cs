using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Models;
using CRM.Data.Types;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services
{
  public class RegisterService : IRegisterService
  {
    private readonly HttpOptions _httpOptions;
    private readonly IRegisterStore _registerStore;
    private readonly IHesherService _hashPassword;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterService> _logger;
    private User? _user { get; set; }

    public RegisterService(
      IOptions<HttpOptions> httpOptions,
      IRegisterStore registerStore,
      IHesherService hashPassword,
      IEmailService emailService,
      ILogger<RegisterService> logger
      )
    {
      _httpOptions = httpOptions.Value;
      _registerStore = registerStore;
      _hashPassword = hashPassword;
      _emailService = emailService;
      _logger = logger;
    }

    public void AddToModel(RegisterRequest request)
    {
      try
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
          RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, string.Empty, ex);
      }
    }

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

    public async Task UserIsRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _registerStore.FindUserByEmail(_user.Email);
    }

    public void GetHash()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(_user.Password, saltArray);
      _user.Password = hash;
    }

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
        IsConfirmed = true, // важно удалить!
        RegistrationDate = _user.RegistrationDate
      };
      await _registerStore.SaveNewUser(user);
    }

    public async Task RemoveUser()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _registerStore.RemoveUser(_user.Email);
    }

    public async Task SendEmailConfirmRegister()
    {
      try
      {
        if (_user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");

        byte[] bytes = Encoding.UTF8.GetBytes(_user.Email);
        string code = Convert.ToBase64String(bytes);

        string url = _httpOptions.Protocol + "://" + _httpOptions.Domaine + ":" +
          _httpOptions.Port + "/Api/Auth/ConfirmRegister/" + code;

        await _emailService.SendEmailConfirmRegister(_user.FirstName, _user.Email, code, url);
      }
      catch (CustomException ex)
      {
        if (ex.Message == "Email exception")
        {
          if (_user != null)
            await _registerStore.RemoveUser(_user.Email);
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        }
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Send email exception", ex);
      }
    }

    public void FromBase64ToString(string input)
    {
      if (string.IsNullOrEmpty(input))
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      byte[] bytes = Convert.FromBase64String(input);
      string result = Encoding.UTF8.GetString(bytes);
      _user = new User { Email = result };
    }

    public void ValidationEmail()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      bool isSuccess = RegExHelper.ChackString(_user.Email, RegExPatterns.Email);
      if (!isSuccess)
        throw new CustomException(ErrorTypes.BadRequest, "Code is invalid");
    }

    public async Task ConfirmRegister()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _registerStore.ConfirmRegister(_user.Email);
    }
  }
}
