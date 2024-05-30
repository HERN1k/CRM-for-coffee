using CRM.Core.Entities;
using CRM.Data.Types;

using LogLib.Types;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Stores
{
  public class SignInStore : ISignInStore
  {
    private readonly AppDBContext _context;
    private readonly ILoggerLib _logger;

    public SignInStore(
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
        var user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        return user;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return null;
      }
    }

    public async Task<bool> SaveToken(string email, string token)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return false;

        bool checkToken = await СheckToken(user.Id);

        var entity = new EntityRefreshToken
        {
          RefreshTokenString = token
        };
        user.RefreshToken = entity;
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }

    private async Task<bool> СheckToken(Guid id)
    {
      try
      {
        var token = await _context.RefreshTokens
          .Where((entity) => entity.UserId == id)
          .SingleOrDefaultAsync();
        if (token != null)
        {
          _context.RefreshTokens.Remove(token);
          await _context.SaveChangesAsync();
        }
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
