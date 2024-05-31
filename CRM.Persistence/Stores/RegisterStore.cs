using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Data.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Stores
{
  public class RegisterStore : IRegisterStore
  {
    private readonly AppDBContext _context;
    private readonly ILogger<RegisterStore> _logger;

    public RegisterStore(
        AppDBContext context,
        ILogger<RegisterStore> logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task FindUserByEmail(string email)
    {
      try
      {
        bool user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .AnyAsync();
        if (user)
          throw new CustomException(ErrorTypes.BadRequest, "The user has already been registered");
      }
      catch (CustomException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task SaveNewUser(EntityUser newUser)
    {
      try
      {
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task RemoveUser(string email)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task ConfirmRegister(string email)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        user.IsConfirmed = true;
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