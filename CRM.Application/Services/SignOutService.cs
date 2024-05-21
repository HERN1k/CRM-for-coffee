using CRM.Application.Types;
using CRM.Data.Types;

namespace CRM.Application.Services
{
  public class SignOutService : ISignOutService
  {
    private readonly ITokenService _tokenService;
    private readonly ISignOutStore _signOutStore;

    public SignOutService(
        ITokenService tokenService,
        ISignOutStore signOutStore
      )
    {
      _tokenService = tokenService;
      _signOutStore = signOutStore;
    }

    public string TokenDecryption(string token)
    {
      return _tokenService.TokenDecryption(token);
    }

    public async Task<bool> RemoveToken(string email, string refreshToken)
    {
      return await _signOutStore.RemoveToken(email, refreshToken);
    }
  }
}
