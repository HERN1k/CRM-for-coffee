using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Models;
using CRM.Data.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Stores
{
  public class AuthRecoveryStore : IAuthRecoveryStore
  {
    private readonly AppDBContext _context;
    private readonly ILogger<AuthRecoveryStore> _logger;

    public AuthRecoveryStore(
        AppDBContext context,
        ILogger<AuthRecoveryStore> logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<User> FindUserByEmail(string email)
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
          throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
        var user = new User
        {
          Id = _user.Id,
          Email = _user.Email,
          Post = _user.Post,
          PhoneNumber = _user.PhoneNumber,
          RegistrationDate = _user.RegistrationDate
        };
        return user;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task SaveNewPassword(Guid id, string hash)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Id == id)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        user.Password = hash;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
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
  }
}