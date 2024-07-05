using AutoMapper;

using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Mapper.Orders;
using CRM.Core.Models;

namespace CRM.Core.Mapper
{
  public class MapperProfile : Profile
  {
    public MapperProfile()
    {
      CreateMap<User, EntityUser>();
      CreateMap<EntityUser, User>();
      CreateMap<RegistrationRequest, User>();
      CreateMap<EntityProductCategory, ProductCategory>();
      CreateMap<EntityProduct, Product>();
      CreateMap<EntityAddOn, AddOn>();
      CreateMap<ProductCategory, EntityProductCategory>();
      CreateMap<Product, EntityProduct>();
      CreateMap<AddOn, EntityAddOn>();
      CreateMap<Order, EntityOrder>().ConvertUsing<ToEntityOrderMapper>();
      CreateMap<EntityOrder, Order>().ConvertUsing<ToOrderMapper>();
    }
  }
}