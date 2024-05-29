using CRM.Application.Types;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CRM.Application.Hubs
{
  [Authorize(Policy = "AdminOrLower")]
  public class ChackoutHub : Hub, IChackoutHub
  {
    public ChackoutHub()
    { }

    public override async Task OnConnectedAsync()
    {
      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
      if (ex != null)
      {
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine($"InnerException: {ex.InnerException}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
      }
      await base.OnDisconnectedAsync(ex);
    }

    [Authorize(Policy = "AdminOrLower")]
    public async Task ResendMessage(string message)
    {
      Console.WriteLine($"Message: {message}");
      await Clients.Caller.SendAsync("resendMassage", message);
    }
  }
}
