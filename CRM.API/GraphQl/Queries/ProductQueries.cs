using CRM.Application.Types;
using CRM.Core.Entities;
using CRM.Data;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  [Authorize(Policy = "AdminOrLower")]
  public class ProductQueries
  {
    private readonly IProductService _productService;

    public ProductQueries(
        IProductService productService
      )
    {
      _productService = productService;
    }

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<EntityProductCategory> GetCategorys([Service(ServiceKind.Synchronized)] AppDBContext context)
    {
      return _productService.GetProductCategory(context);
    }
  }
}