using System.Text.RegularExpressions;

using CRM.Application.GraphQl.Dto;
using CRM.Application.RegEx;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions;

namespace CRM.Application.RequestValidation
{
  /// <summary>
  ///   This class checks the validity of the request data
  /// </summary>
  public static class RequestValidator
  {
    /// <summary>
    ///   Validation of a request to register a new worker
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(RegisterRequest request)
    {
      bool firstName = Regex.IsMatch(request.FirstName, RegExPatterns.FirstName);
      if (!firstName)
        throw new CustomException(ErrorTypes.ValidationError, "First name is incorrect or null");

      bool lastName = Regex.IsMatch(request.LastName, RegExPatterns.LastName);
      if (!lastName)
        throw new CustomException(ErrorTypes.ValidationError, "Last name is incorrect or null");

      bool fatherName = Regex.IsMatch(request.FatherName, RegExPatterns.FatherName);
      if (!fatherName)
        throw new CustomException(ErrorTypes.ValidationError, "Father name is incorrect or null");

      bool password = Regex.IsMatch(request.Password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");

      if (request.Age < 16 || request.Age > 65)
        throw new CustomException(ErrorTypes.ValidationError, "Age is incorrect or null");

      bool gender = Regex.IsMatch(request.Gender, RegExPatterns.Gender);
      if (!gender)
        throw new CustomException(ErrorTypes.ValidationError, "Gender is incorrect or null");

      bool phoneNumber = Regex.IsMatch(request.PhoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        throw new CustomException(ErrorTypes.ValidationError, "Phone number is incorrect or null");

      bool email = Regex.IsMatch(request.Email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool post = Regex.IsMatch(request.Post, RegExPatterns.Post);
      if (!post)
        throw new CustomException(ErrorTypes.ValidationError, "Post is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to sign in
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(SignInRequest request)
    {
      bool email = Regex.IsMatch(request.Email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool password = Regex.IsMatch(request.Password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to update password
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(UpdatePasswordRequest request)
    {
      bool email = Regex.IsMatch(request.Email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool password = Regex.IsMatch(request.Password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");

      bool newPassword = Regex.IsMatch(request.NewPassword, RegExPatterns.Password);
      if (!newPassword)
        throw new CustomException(ErrorTypes.ValidationError, "New password is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to recovery password
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(RecoveryPasswordRequest request)
    {
      bool email = Regex.IsMatch(request.Email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool phoneNumber = Regex.IsMatch(request.PhoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        throw new CustomException(ErrorTypes.ValidationError, "Phone number is incorrect or null");

      bool post = Regex.IsMatch(request.Post, RegExPatterns.Post);
      if (!post)
        throw new CustomException(ErrorTypes.ValidationError, "Post is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to add new product category
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(ProductCategoryRequest request)
    {
      bool isValidName = Regex.IsMatch(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidImage = Regex.IsMatch(request.Image, RegExPatterns.URL);
      if (!isValidImage)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to add new product
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(ProductRequest request)
    {
      bool isValidName = Regex.IsMatch(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidCategoryName = Regex.IsMatch(request.CategoryName, RegExPatterns.ProductName);
      if (!isValidCategoryName)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      bool isValidImage = Regex.IsMatch(request.Image, RegExPatterns.URL);
      if (!isValidImage)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      if (request.Price < 0)
        throw new CustomException(ErrorTypes.ValidationError, "Price is incorrect or null");

      if (request.Amount < 1)
        throw new CustomException(ErrorTypes.ValidationError, "Amount is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to add new addOn
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(AddOnRequest request)
    {
      bool isValidName = Regex.IsMatch(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidProductName = Regex.IsMatch(request.ProductName, RegExPatterns.ProductName);
      if (!isValidProductName)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      if (request.Price < 0)
        throw new CustomException(ErrorTypes.ValidationError, "Price is incorrect or null");

      if (request.Amount <= 0 && request.Amount >= 10)
        throw new CustomException(ErrorTypes.ValidationError, "Amount is incorrect or null");
    }

    /// <summary>
    ///   Validation of a request to create order
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void Validate(CreateOrderRequest request)
    {
      bool paymentMethod = Enum.IsDefined(typeof(PaymentMethods), request.PaymentMethod);
      if (!paymentMethod)
        throw new CustomException(ErrorTypes.ValidationError, "Payment method is incorrect or null");

      bool workerEmail = Regex.IsMatch(request.WorkerEmail, RegExPatterns.Email);
      if (!workerEmail)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      foreach (var product in request.Products)
      {
        if (product.Amount <= 0)
          throw new CustomException(ErrorTypes.ValidationError, "Product amount is incorrect");

        if (product.AddOns.Count > 0)
        {
          foreach (var addOn in product.AddOns)
          {
            if (addOn.Amount <= 0)
              throw new CustomException(ErrorTypes.ValidationError, "Addon amount is incorrect");
          }
        }
      }
    }

    /// <summary>
    ///   Validates the given email address against a predefined pattern
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <exception cref="CustomException"></exception>
    public static void ValidateEmail(string email)
    {
      bool isSuccess = Regex.IsMatch(email, RegExPatterns.Email);
      if (!isSuccess)
        throw new CustomException(ErrorTypes.BadRequest, "Code is invalid");
    }

    /// <summary>
    ///   Validation of a request to add new addOn
    /// </summary>
    /// <exception cref="CustomException"></exception>
    public static void ValidateProductsName(params string[] names)
    {
      foreach (var name in names)
      {
        bool isValidName = Regex.IsMatch(name, RegExPatterns.ProductName);
        if (!isValidName)
          throw new CustomException(ErrorTypes.ValidationError, $"Name: '{name}' is incorrect or null");
      }
    }
  }
}