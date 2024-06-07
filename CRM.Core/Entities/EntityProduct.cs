﻿using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityProduct : BaseProductEntity
  {
    public string Image { get; set; } = string.Empty;
    public Guid ProductCategoryId { get; set; }
    public EntityProductCategory? ProductCategory { get; set; }
    public List<EntityAddOn>? AddOns { get; set; }
  }
}