using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.OrderTypes
{
  public class OrderAddOnType : ObjectType<EntityOrderAddOn>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityOrderAddOn> descriptor)
    {
      descriptor.Ignore(e => e.OrderProduct);
    }
  }
}