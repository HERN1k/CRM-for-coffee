using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthRepository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Repositories
{
  public class SignOutRepository : ISignOutRepository
  {
    private readonly AppDBContext _context;
    private readonly ILogger<SignOutRepository> _logger;

    public SignOutRepository(
        AppDBContext context,
        ILogger<SignOutRepository> logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task RemoveToken(string email, string refreshToken)
    {
      try
      {
        var user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        var token = new EntityRefreshToken { Id = user.Id };
        _context.Entry(token).State = EntityState.Deleted;

        await _context.SaveChangesAsync();
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
