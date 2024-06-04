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
    private readonly IRepository<EntityUser> _userRepository;
    private readonly IRepository<EntityRefreshToken> _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public SignOutService(
        IRepository<EntityUser> userRepository,
        IRepository<EntityRefreshToken> refreshTokenRepository,
        ITokenService tokenService
      )
    {
      _userRepository = userRepository;
      _refreshTokenRepository = refreshTokenRepository;
      _tokenService = tokenService;
    }

    public string TokenDecryption(string token)
    {
      return _tokenService.TokenDecryption(token);
    }

    public async Task RemoveToken(string email, string refreshToken)
    {
      var user = await _userRepository.FindSingleAsync(e => e.Email == email);
      if (user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      var removeToken = await _refreshTokenRepository.FindSingleAsync(e => e.Id == user.Id);
      if (removeToken == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _refreshTokenRepository.RemoveAsync(removeToken);
    }
  }
}