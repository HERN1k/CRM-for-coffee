namespace CRM.Core.Interfaces.Settings
{
  public class EmailSettings
  {
    public string Address { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
  }
}
