using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.ProductTypes
{
  public class AddOnType : ObjectType<EntityAddOn>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityAddOn> descriptor)
    {
      descriptor.Ignore(e => e.ProductId);
      descriptor.Ignore(e => e.Product);
    }
  }
}