using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CRM.Application.Types;
using CRM.Application.Types.Options;

using LogLib.Types;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Application.Security
{
  public class TokenService : ITokenService
  {
    private readonly JwtOptions _jwtOptions;
    private readonly ILoggerLib _logger;

    public TokenService(
        IOptions<JwtOptions> jwtOptions,
        ILoggerLib logger
      )
    {
      _jwtOptions = jwtOptions.Value;
      _logger = logger;
    }

    public string CreateJwtToken(List<Claim> claims, int time)
    {
      var jwt = new JwtSecurityToken(
              issuer: _jwtOptions.IssuerJwt,
              audience: _jwtOptions.AudienceJwt,
              claims: claims,
              expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(time)),
              signingCredentials: new SigningCredentials(
                  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKeyJwt)),
                  SecurityAlgorithms.HmacSha256
                ));

      return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string TokenDecryption(string token)
    {
      string result = string.Empty;
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Payload.Claims.ToList();
        foreach (var claim in claims)
        {
          if (claim.Type == ClaimTypes.Email)
            result = claim.Value;
        }
      }
      catch (Exception ex)
      {
        _logger.WriteErrorLog(ex);
      }
      return result;
    }

    public async Task<bool> ValidateToken(string token)
    {
      try
      {
        var securityToken = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = _jwtOptions.IssuerJwt,
          ValidateAudience = true,
          ValidAudience = _jwtOptions.AudienceJwt,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecurityKeyJwt)
              ),
          ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewJwt)
        };
        var refreshValidate = await securityToken.ValidateTokenAsync(token, validationParameters);
        return refreshValidate.IsValid;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }
  }
}