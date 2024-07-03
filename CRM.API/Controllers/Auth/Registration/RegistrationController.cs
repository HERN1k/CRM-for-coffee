using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.Services.AuthServices.Registration;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth.Registration
{
    [ApiController]
    [Route("api/auth")]
    public class RegistrationController(
        IRegistrationService registrationServices
      ) : ControllerBase
    {
        private readonly IRegistrationService _registrationService = registrationServices;

        [SwaggerOperation(
          Summary = "Registration a new user.",
          OperationId = "Registration",
          Tags = ["Registration"]
        )]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, null, typeof(ExceptionResponse))]
        [SwaggerResponse(500, null, typeof(ExceptionResponse))]
        [HttpPost("registration")]
        [Authorize(Policy = "ManagerOrUpper")]
        public async Task<IActionResult> Registration(RegistrationRequest request) =>
          await _registrationService.RegistrationNewWorkerAsync(request);

        [SwaggerOperation(
          Summary = "Confirmation of registration.",
          OperationId = "ConfirmEmail",
          Tags = ["Registration"]
        )]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, null, typeof(ExceptionResponse))]
        [SwaggerResponse(500, null, typeof(ExceptionResponse))]
        [HttpGet("confirm_email/{code}")]
        public async Task<IActionResult> ConfirmEmail(string code) =>
          await _registrationService.ConfirmEmailAsync(code);
    }
}