using CRM.Application.Tools.RequestValidation;
using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Repositories.AuthRepositories.AuthRecovery;
using CRM.Core.Interfaces.Services.AuthServices.AuthRecovery;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices.AuthRecovery
{
  public class AuthRecoveryService(
      IAuthRecoveryRepository repository,
      IAuthRecoveryComponents components,
      IEmailService emailService
    ) : IAuthRecoveryService
  {
    private readonly IAuthRecoveryRepository _repository = repository;
    private readonly IAuthRecoveryComponents _components = components;
    private readonly IEmailService _emailService = emailService;

    public async Task<IActionResult> RecoveryPasswordAsync(HttpContext httpContext, RecoveryPasswordRequest request)
    {
      RequestValidator.Validate(request);

      User user = await _repository.FindWorker(request.Email, ErrorTypes.BadRequest, "The user is not registered");

      _components.СomparisonRecoveryPassData(user, request);

      string newPassword = _components.GetNewPassword(16);

      await _emailService.SendRecoveryPasswordEmail(user, newPassword);

      string hash = HesherService.GetPasswordHash(user, newPassword);

      await _repository.SaveNewPassword(user.Id, hash);

      await _repository.RemoveRefreshToken(user.Id);

      _components.SetUnauthorizedCookies(httpContext);

      return new OkResult();
    }
  }
}