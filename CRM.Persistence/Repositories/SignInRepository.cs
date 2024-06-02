using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthRepository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Repositories
{
  public class SignInRepository : ISignInRepository
  {
    private readonly AppDBContext _context;
    private readonly ILogger<SignInRepository> _logger;

    public SignInRepository(
        AppDBContext context,
        ILogger<SignInRepository> logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<EntityUser> FindUserByEmail(string email)
    {
      try
      {
        var user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.BadRequest, "The user is not registered");
        return user;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        _logger.LogWarning(ex.StackTrace);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    public async Task SaveToken(string email, string token)
    {
      try
      {
        var user = await _context.Users
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");

        await СheckToken(user.Id);

        var entity = new EntityRefreshToken
        {
          RefreshTokenString = token
        };
        user.RefreshToken = entity;
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        _logger.LogWarning(ex.StackTrace);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }

    private async Task СheckToken(Guid id)
    {
      try
      {
        var token = await _context.RefreshTokens
          .Where((entity) => entity.Id == id)
          .SingleOrDefaultAsync();
        if (token != null)
        {
          _context.RefreshTokens.Remove(token);
          await _context.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        _logger.LogWarning(ex.StackTrace);
        throw new CustomException(ErrorTypes.ServerError, "DB exception", ex);
      }
    }
  }
}