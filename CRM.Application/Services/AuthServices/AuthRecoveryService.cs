using System.Text.RegularExpressions;

using CRM.Application.RegEx;
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
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices
{
  public class AuthRecoveryService(
      IRepository repository,
      IEmailSender emailSender
    ) : IAuthRecoveryService
  {
    private readonly IRepository _repository = repository;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<IActionResult> RecoveryPasswordAsync(HttpContext httpContext, RecoveryPasswordRequest request)
    {
      RequestValidator.Validate(request);

      EntityUser entityUser = await FindUserFromDB(request.Email, ErrorTypes.BadRequest, "The user is not registered");

      User user = RequestMapper.MapToModel(entityUser);

      СomparisonRecoveryPassData(user, request);

      string newPassword = GetNewPassword(16);

      await _emailSender.SendRecoveryPasswordEmail(user, newPassword);

      string hash = HesherService.GetPasswordHash(user, newPassword);

      await SaveNewPassword(user, hash);

      await RemoveRefreshToken(user);

      SetUnauthorizedCookies(httpContext);

      return new OkResult();
    }

    private async Task<EntityUser> FindUserFromDB(string email, ErrorTypes type, string errorMassage)
    {
      var result = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, errorMassage);
      return result;
    }
    private void СomparisonRecoveryPassData(User user, RecoveryPasswordRequest request)
    {
      if (user.PhoneNumber != request.PhoneNumber)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
      if (user.Post != request.Post)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
    }
    private string GetNewPassword(int length)
    {
      string newPassword;
      bool isCorrect;
      int iterations = 0;
      do
      {
        newPassword = HesherService.GetRandomPassword(length);
        isCorrect = Regex.IsMatch(newPassword, RegExPatterns.Password);
        iterations++;
      } while (!(isCorrect || iterations >= 10));

      if (string.IsNullOrEmpty(newPassword) || iterations >= 10)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      return newPassword;
    }
    private async Task SaveNewPassword(User user, string hash)
    {
      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Id == user.Id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      entityUser.Password = hash;

      await _repository.UpdateAsync<EntityUser>(entityUser);
    }
    private async Task RemoveRefreshToken(User user)
    {
      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == user.Id);
    }
    private void SetUnauthorizedCookies(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }
  }
}