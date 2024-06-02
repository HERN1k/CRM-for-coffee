using CRM.Core.Interfaces.AuthRepository;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.JwtToken;

namespace CRM.Application.Services.AuthServices
{
    public class SignOutService : ISignOutService
    {
        private readonly ITokenService _tokenService;
        private readonly ISignOutRepository _signOutRepository;

        public SignOutService(
            ITokenService tokenService,
            ISignOutRepository signOutRepository
          )
        {
            _tokenService = tokenService;
            _signOutRepository = signOutRepository;
        }

        public string TokenDecryption(string token)
        {
            return _tokenService.TokenDecryption(token);
        }

        public async Task RemoveToken(string email, string refreshToken)
        {
            await _signOutRepository.RemoveToken(email, refreshToken);
        }
    }
}