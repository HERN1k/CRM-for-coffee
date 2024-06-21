using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.Repositories;

namespace CRM.Application.Services.AuthServices
{
  public class SignOutService : ISignOutService
  {
    private readonly IRepository _repository;
    private readonly ITokenService _tokenService;

    public SignOutService(
        IRepository repository,
        ITokenService tokenService
      )
    {
      _repository = repository;
      _tokenService = tokenService;
    }

    public string TokenDecryption(string token)
    {
      return _tokenService.TokenDecryption(token);
    }

    public async Task RemoveToken(string email, string refreshToken)
    {
      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == email);
      if (user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == user.Id);
    }
  }
}