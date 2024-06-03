﻿namespace CRM.Core.Models
{
  public class AddOn
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
  }
}