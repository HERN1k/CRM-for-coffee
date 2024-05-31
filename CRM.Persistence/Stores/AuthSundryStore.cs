using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Data.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Stores
{
  public class AuthSundryStore : IAuthSundryStore
  {
    private readonly AppDBContext _context;
    private readonly ILogger<AuthSundryStore> _logger;

    public AuthSundryStore(
        AppDBContext context,
        ILogger<AuthSundryStore> logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<EntityUser> FindUserByEmail(string email)
    {
      try
      {
        var userData = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .Select((entity) => new
          {
            entity.Id,
            entity.FirstName,
            entity.Email,
            entity.Post,
            entity.Password,
            entity.RegistrationDate
          })
          .SingleOrDefaultAsync();
        if (userData == null)
          throw new Exception("Exception");
        var user = new EntityUser
        {
          Id = userData.Id,
          FirstName = userData.FirstName,
          Email = userData.Email,
          Post = userData.Post,
          Password = userData.Password,
          RegistrationDate = userData.RegistrationDate
        };
        return user;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new Exception("Exception");
      }
    }

    public async Task<string> FindTokenById(Guid id)
    {
      try
      {
        var token = await _context.RefreshTokens
          .AsNoTracking()
          .Where((entity) => entity.UserId == id)
          .Select((entity) => new { entity.RefreshTokenString })
          .SingleOrDefaultAsync();
        if (token == null)
          throw new Exception("Exception");
        return token.RefreshTokenString;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new Exception("Exception");
      }
    }

    public async Task RemoveRefreshToken(Guid id)
    {
      try
      {
        var token = new EntityRefreshToken { UserId = id };
        _context.Entry(token).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task SaveNewPassword(string email, string password)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        user.Password = password;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }
  }
}