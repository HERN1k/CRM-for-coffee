using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.OrderTypes
{
  public class OrderType : ObjectType<EntityOrder>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityOrder> descriptor)
    {

    }
  }
}