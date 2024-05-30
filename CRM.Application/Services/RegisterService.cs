using System.Text;
using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Models;
using CRM.Data.Types;

using Microsoft.Extensions.Options;

namespace CRM.Application.Services
{
    public class RegisterService : IRegisterService
  {
    private readonly HttpOptions _httpOptions;
    private readonly IRegisterStore _registerStore;
    private readonly IHesherService _hashPassword;
    private readonly IEmailService _emailService;
    private User? _user { get; set; }

    public RegisterService(
      IOptions<HttpOptions> httpOptions,
      IRegisterStore registerStore,
      IHesherService hashPassword,
      IEmailService emailService
      )
    {
      _httpOptions = httpOptions.Value;
      _registerStore = registerStore;
      _hashPassword = hashPassword;
      _emailService = emailService;
    }

    public bool AddToModel(RegisterRequest request)
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
      return true;
    }

    public ValidationResult ValidationDataRegister()
    {
      if (_user == null)
        return new ValidationResult { IsSuccess = false, Field = "_user" };

      bool firstName = RegExHelper.ChackString(_user.FirstName, RegExPatterns.FirstName);
      if (!firstName)
        return new ValidationResult { IsSuccess = false, Field = "FirstName" };

      bool lastName = RegExHelper.ChackString(_user.LastName, RegExPatterns.LastName);
      if (!lastName)
        return new ValidationResult { IsSuccess = false, Field = "LastName" };

      bool fatherName = RegExHelper.ChackString(_user.FatherName, RegExPatterns.FatherName);
      if (!fatherName)
        return new ValidationResult { IsSuccess = false, Field = "FatherName" };

      bool password = RegExHelper.ChackString(_user.Password, RegExPatterns.Password);
      if (!password)
        return new ValidationResult { IsSuccess = false, Field = "Password" };

      if (_user.Age < 16 || _user.Age > 65)
        return new ValidationResult { IsSuccess = false, Field = "Age" };

      bool gender = RegExHelper.ChackString(_user.Gender, RegExPatterns.Gender);
      if (!gender)
        return new ValidationResult { IsSuccess = false, Field = "Gender" };

      bool phoneNumber = RegExHelper.ChackString(_user.PhoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        return new ValidationResult { IsSuccess = false, Field = "PhoneNumber" };

      bool email = RegExHelper.ChackString(_user.Email, RegExPatterns.Email);
      if (!email)
        return new ValidationResult { IsSuccess = false, Field = "Email" };

      bool post = RegExHelper.ChackString(_user.Post, RegExPatterns.Post);
      if (!post)
        return new ValidationResult { IsSuccess = false, Field = "Post" };

      return new ValidationResult { IsSuccess = true, Field = string.Empty };
    }

    public async Task<bool> UserIsRegister()
    {
      if (_user == null)
        return false;
      bool result = await _registerStore.FindUserByEmail(_user.Email);
      return result;
    }

    public bool GetHash()
    {
      if (_user == null)
        return false;
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(_user.Password, saltArray);
      _user.Password = hash;
      return true;
    }

    public async Task<bool> SaveNewUser()
    {
      if (_user == null)
        return false;
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
      return await _registerStore.SaveNewUser(user);
    }

    public async Task<bool> RemoveUser()
    {
      if (_user == null)
        return false;
      bool result = await _registerStore.RemoveUser(_user.Email);
      return result;
    }

    public async Task<bool> SendEmailConfirmRegister()
    {
      if (_user == null)
        return false;

      byte[] bytes = Encoding.UTF8.GetBytes(_user.Email);
      string code = Convert.ToBase64String(bytes);

      string url = _httpOptions.Protocol + "://" + _httpOptions.Domaine + ":" +
        _httpOptions.Port + "/Api/Auth/ConfirmRegister/" + code;

      return await _emailService.SendEmailConfirmRegister(_user.FirstName, _user.Email, code, url);
    }

    public bool FromBase64ToString(string input)
    {
      if (string.IsNullOrEmpty(input))
        return false;
      byte[] bytes = Convert.FromBase64String(input);
      string result = Encoding.UTF8.GetString(bytes);
      _user = new User { Email = result };
      return true;
    }

    public bool ValidationEmail()
    {
      if (_user == null)
        return false;
      return RegExHelper.ChackString(_user.Email, RegExPatterns.Email);
    }

    public async Task<bool> ConfirmRegister()
    {
      if (_user == null)
        return false;
      bool result = await _registerStore.ConfirmRegister(_user.Email);
      return result;
    }
  }
}
