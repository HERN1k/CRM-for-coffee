using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Data.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Stores
{
  public class SignOutStore : ISignOutStore
  {
    private readonly AppDBContext _context;
    private readonly ILogger<SignOutStore> _logger;

    public SignOutStore(
        AppDBContext context,
        ILogger<SignOutStore> logger
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
        var token = new EntityRefreshToken { UserId = user.Id };
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
