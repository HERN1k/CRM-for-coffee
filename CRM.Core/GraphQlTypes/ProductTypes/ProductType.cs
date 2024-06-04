using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.ProductTypes
{
  public class ProductType : ObjectType<EntityProduct>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityProduct> descriptor)
    {
      descriptor.Ignore(e => e.ProductCategoryId);
      descriptor.Ignore(e => e.ProductCategory);
      descriptor.Ignore(e => e.AddOns);
    }
  }
}