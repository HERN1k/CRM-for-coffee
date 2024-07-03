using CRM.Application.Tools.RequestDataMapper;
using CRM.Application.Tools.RequestValidation;
using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Repositories.AuthSundry;
using CRM.Core.Interfaces.Services.AuthServices.AuthSundry;
using CRM.Core.Interfaces.Tools.Security.JwtToken;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices.AuthSundry
{
    public class AuthSundryService(
      IAuthSundryRepository repository,
      IAuthSundryComponents components,
      ITokenService tokenService,
      IEmailService emailService
    ) : IAuthSundryService
  {
    private readonly IAuthSundryRepository _repository = repository;
    private readonly IAuthSundryComponents _components = components;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailService _emailService = emailService;

    public async Task<IActionResult> ChangeTokenAsync(HttpContext httpContext, ChangeTokenRequest request)
    {
      try
      {
        string token = _components.GetRefreshToken(httpContext);

        RequestValidator.ValidateEmail(request.Email);

        await _tokenService.ValidateToken(token);

        EntityUser entityUser = await _repository.FindWorker(request.Email, ErrorTypes.Unauthorized, "Unauthorized");

        User user = RequestMapper.MapToModel(entityUser);

        await _repository.CheckImmutableToken(user.Id, token);

        string accessToken = _tokenService.GetJwtToken(user, TokenTypes.Access);

        _components.SetAccessTokenCookie(httpContext, accessToken);

        return new OkResult();
      }
      catch (CustomException ex)
      {
        _components.SetUnauthorizedCookies(httpContext);
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
      catch (Exception ex)
      {
        _components.SetUnauthorizedCookies(httpContext);
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
    }

    public async Task<IActionResult> UpdatePasswordAsync(HttpContext httpContext, UpdatePasswordRequest request)
    {
      RequestValidator.Validate(request);

      EntityUser entityUser = await _repository.FindWorker(request.Email, ErrorTypes.ServerError, "Server error");

      User user = RequestMapper.MapToModel(entityUser);

      _components.CheckPasswordDifference(request);

      _components.PasswordСheck(user, request.Password);

      string hash = HesherService.GetPasswordHash(user, request.Password);

      await _repository.SaveNewPassword(user.Id, hash);

      await _repository.RemoveRefreshToken(user.Id);

      await _emailService.SendUpdatePasswordEmail(user);

      _components.SetUnauthorizedCookies(httpContext);

      return new OkResult();
    }
  }
}