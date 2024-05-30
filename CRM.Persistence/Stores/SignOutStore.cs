using CRM.Core.Entities;
using CRM.Data.Types;

using LogLib.Types;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Stores
{
  public class SignOutStore : ISignOutStore
  {
    private readonly AppDBContext _context;
    private readonly ILoggerLib _logger;

    public SignOutStore(
        AppDBContext context,
        ILoggerLib logger
      )
    {
      _context = context;
      _logger = logger;
    }

    public async Task<bool> RemoveToken(string email, string refreshToken)
    {
      try
      {
        var user = await _context.Users
          .AsNoTracking()
          .Where((entity) => entity.Email == email)
          .SingleOrDefaultAsync();
        if (user == null)
          return false;
        var token = new EntityRefreshToken { UserId = user.Id };
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
