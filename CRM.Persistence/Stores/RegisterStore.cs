using CRM.Core.Entities;
using CRM.Data.Types;

using LogLib.Types;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Stores
{
  public class RegisterStore : IRegisterStore
  {
    private readonly AppDBContext _context;
    private readonly ILoggerLib _logger;

    public RegisterStore(
        AppDBContext context,
        ILoggerLib logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<bool> FindUserByEmail(string email)
    {
      try
      {
        bool user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .AnyAsync();
        return user;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }

    public async Task<bool> SaveNewUser(EntityUser newUser)
    {
      try
      {
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }

    public async Task<bool> RemoveUser(string email)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return true;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
    }

    public async Task<bool> ConfirmRegister(string email)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return false;
        user.IsConfirmed = true;
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
