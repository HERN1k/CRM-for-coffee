using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.OrderTypes
{
  public class OrderProductType : ObjectType<EntityOrderProduct>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityOrderProduct> descriptor)
    {
      descriptor.Ignore(e => e.Order);
    }
  }
}