using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Application.Security
{
  public class TokenService(
      IOptions<JwtSettings> jwtSettings,
      ILogger<TokenService> logger
    ) : ITokenService
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ILogger<TokenService> _logger = logger;

    public string GetJwtToken(User user, TokenTypes typesTokens)
    {
      int tokenLifetime = 30;
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Post),
      };

      if ((int)typesTokens == 1)
        tokenLifetime = _jwtSettings.AccessTokenLifetime;
      else if ((int)typesTokens == 2)
        tokenLifetime = _jwtSettings.RefreshTokenLifetime;

      return CreateJwtToken(claims, tokenLifetime);
    }

    public JwtTokenClaims TokenDecryption(string token)
    {
      try
      {
        var result = new JwtTokenClaims();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Payload.Claims.ToList();

        foreach (var claim in claims)
        {
          if (claim.Type == ClaimTypes.NameIdentifier)
            result.Id = claim.Value;

          if (claim.Type == ClaimTypes.Email)
            result.Email = claim.Value;

          if (claim.Type == ClaimTypes.Role)
            result.Post = claim.Value;
        }

        if (string.IsNullOrEmpty(result.Id) || string.IsNullOrEmpty(result.Email) || string.IsNullOrEmpty(result.Post))
          throw new CustomException(ErrorTypes.BadRequest, "Token not found");

        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Token decryption error", ex);
      }
    }

    public async Task ValidateToken(string token)
    {
      try
      {
        var securityToken = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = _jwtSettings.IssuerJwt,
          ValidateAudience = true,
          ValidAudience = _jwtSettings.AudienceJwt,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecurityKeyJwt)
              ),
          ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewJwt)
        };
        var refreshValidate = await securityToken.ValidateTokenAsync(token, validationParameters);
        if (!refreshValidate.IsValid)
          throw new CustomException(ErrorTypes.BadRequest, "Token is not valid");
      }
      catch (CustomException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      }
    }

    private string CreateJwtToken(List<Claim> claims, int time)
    {
      var jwt = new JwtSecurityToken(
              issuer: _jwtSettings.IssuerJwt,
              audience: _jwtSettings.AudienceJwt,
              claims: claims,
              expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(time)),
              signingCredentials: new SigningCredentials(
                  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKeyJwt)),
                  SecurityAlgorithms.HmacSha256
                ));

      return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
  }
}