using CRM.Core.Entities;
using CRM.Core.Models;
using CRM.Data.Types;

using LogLib.Types;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Stores
{
  public class AuthRecoveryStore : IAuthRecoveryStore
  {
    private readonly AppDBContext _context;
    private readonly ILoggerLib _logger;

    public AuthRecoveryStore(
        AppDBContext context,
        ILoggerLib logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<User?> FindUserByEmail(string email)
    {
      try
      {
        var _user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .Select((entity) => new
          {
            entity.Id,
            entity.Email,
            entity.Post,
            entity.PhoneNumber,
            entity.RegistrationDate
          })
          .SingleOrDefaultAsync();
        if (_user == null)
          return null;
        var user = new User
        {
          Id = (Guid)_user.Id!,
          Email = _user.Email,
          Post = _user.Post,
          PhoneNumber = _user.PhoneNumber,
          RegistrationDate = _user.RegistrationDate
        };
        return user;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return null;
      }
    }

    public async Task<bool> SaveNewPassword(Guid id, string hash)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Id == id)
          .SingleOrDefaultAsync();
        if (user == null)
          return false;
        user.Password = hash;
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
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
  }
}
