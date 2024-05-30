namespace CRM.Application.GraphQl.Dto
{
  public class ProductRequest
  {
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
  }
}