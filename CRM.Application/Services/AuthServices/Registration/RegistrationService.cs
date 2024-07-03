using CRM.Application.Tools.RequestDataMapper;
using CRM.Application.Tools.RequestValidation;
using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Repositories.Registration;
using CRM.Core.Interfaces.Services.AuthServices.Registration;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices.Registration
{
    public class RegistrationService(
        IOptions<HttpSettings> httpSettings,
        IOptions<EmailConfirmRegisterSettings> emailConfirmRegisterSettings,
        IRegistrationRepository repository,
        IRegistrationComponents components,
        IEmailService emailService
      ) : IRegistrationService
    {
        private readonly HttpSettings _httpSettings = httpSettings.Value;
        private readonly EmailConfirmRegisterSettings _emailConfirmRegisterSettings = emailConfirmRegisterSettings.Value;
        private readonly IRegistrationRepository _repository = repository;
        private readonly IRegistrationComponents _components = components;
        private readonly IEmailService _emailService = emailService;

        public async Task<IActionResult> RegistrationNewWorkerAsync(RegistrationRequest request)
        {
            User user = RequestMapper.MapToModel(request);

            _components.EnsureNonAdminRole(user.Post);

            await _repository.RegistrationСheck(user.Email);

            user.Password = HesherService.GetPasswordHash(user);

            await _emailService.SendEmailConfirmRegisterAsync(user, _httpSettings, _emailConfirmRegisterSettings);

            var newUser = await _repository.SaveNewWorker(user);

            var adminList = _repository.GetAdminList();

            await _emailService.NotifyAdminsOnManagerRegistration(newUser, adminList);

            return new OkResult();
        }

        public async Task<IActionResult> ConfirmEmailAsync(string code)
        {
            User user = new User { Email = _components.GetStringFromBase64(code) };

            RequestValidator.ValidateEmail(user.Email);

            await _repository.CheckingMailConfirmation(user.Email);

            await _repository.SetTrueIsConfirmed(user.Email);

            return new OkResult();
        }
    }
}