﻿using CRM.Core.Entities;

using HotChocolate.Types;

namespace CRM.Core.GraphQlTypes.ProductTypes
{
  public class ProductCategoryType : ObjectType<EntityProductCategory>
  {
    protected override void Configure(IObjectTypeDescriptor<EntityProductCategory> descriptor)
    {
      //descriptor.Field(e => e.Id).Type<NonNullType<IdType>>();
    }
  }
}