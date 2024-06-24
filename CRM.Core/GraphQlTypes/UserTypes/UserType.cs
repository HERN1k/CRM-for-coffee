using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.UserTypes
{
  public class UserType : ObjectType<EntityUser>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityUser> descriptor)
    {
      descriptor.Ignore(e => e.Password);
      descriptor.Ignore(e => e.RefreshToken);
    }
  }
}
