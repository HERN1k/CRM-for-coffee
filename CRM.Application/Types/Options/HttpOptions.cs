namespace CRM.Application.Types.Options
{
  public class HttpOptions
  {
    public string Protocol { get; set; } = string.Empty;
    public string Domaine { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
  }
}
