using CRM.Core.Entities;
using CRM.Data.Types;

using LogLib.Types;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Stores
{
  public class AuthSundryStore : IAuthSundryStore
  {
    private readonly AppDBContext _context;
    private readonly ILoggerLib _logger;

    public AuthSundryStore(
        AppDBContext context,
        ILoggerLib logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<EntityUser?> FindUserByEmail(string email)
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
          return null;
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
        await _logger.WriteErrorLog(ex);
        return null;
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
          return string.Empty;
        return token.RefreshTokenString;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return string.Empty;
      }
    }

    public async Task<bool> RemoveRefreshToken(Guid id)
    {
      try
      {
        var token = new EntityRefreshToken { UserId = id };
        _context.Entry(token).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }

    public async Task<bool> SaveNewPassword(string email, string password)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return false;
        user.Password = password;
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }
  }
}