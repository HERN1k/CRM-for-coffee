namespace CRM.Application.GraphQl.Dto
{
  public class AddOnRequest
  {
    public string ProductName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
  }
}