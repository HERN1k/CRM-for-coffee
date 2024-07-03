using CRM.Application.Tools.RequestDataMapper;
using CRM.Application.Tools.RequestValidation;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Repositories.SignIn;
using CRM.Core.Interfaces.Services.AuthServices.SignIn;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices.SignIn
{
    public class SignInService(
        ISignInRepository repository,
        ISignInComponents components
      ) : ISignInService
    {
        private readonly ISignInRepository _repository = repository;
        private readonly ISignInComponents _components = components;

        public async Task<IActionResult> SignInAsync(HttpContext httpContext, SignInRequest request)
        {
            RequestValidator.Validate(request);

            var entityUser = await _repository.FindWorker(request.Email);

            User user = RequestMapper.MapToModel(entityUser);

            _components.ChackingCorrectPassword(user, request);

            var tokens = _components.CreateJwtTokenDictionary(user);

            await _repository.SaveToken(user.Id, tokens[TokenTypes.Refresh]);

            _components.SetCookie(httpContext, tokens[TokenTypes.Access], tokens[TokenTypes.Refresh]);

            var response = _components.CreateResponse(user, tokens[TokenTypes.Refresh]);

            return new OkObjectResult(response);
        }
    }
}