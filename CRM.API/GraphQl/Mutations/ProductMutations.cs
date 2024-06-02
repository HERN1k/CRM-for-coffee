using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Mutations
{
  [Authorize(Policy = "AdminOrLower")]
  public class ProductMutations
  {
    //[UseProjection]
    //public async Task<EntityProductCategory> CreateCategory(
    //    ProductCategoryRequest category,
    //    [Service] AppDBContext _context
    //  )
    //{
    //  var newCategory = new EntityProductCategory
    //  {
    //    Name = category.Name,
    //    Image = category.Image
    //  };

    //  _context.ProductCategorys.Add(newCategory);
    //  await _context.SaveChangesAsync();
    //  return newCategory;
    //}

    //[UseProjection]
    //public async Task<EntityProduct> CreateProduct(
    //    ProductRequest product,
    //    [Service] AppDBContext _context
    //  )
    //{
    //  var category = await _context.ProductCategorys
    //    .AsNoTracking()
    //    .Where((entity) => entity.Name == product.CategoryName)
    //    .Select((entity) => new
    //    {
    //      entity.Id,
    //      entity.Name
    //    })
    //    .SingleOrDefaultAsync();

    //  if (category == null)
    //    throw new ArgumentException("ParentNotFound");

    //  var newProduct = new EntityProduct
    //  {
    //    Name = product.Name,
    //    Image = product.Image,
    //    Price = product.Price,
    //    Amount = product.Amount,
    //    ProductCategoryId = category.Id
    //  };

    //  _context.Products.Add(newProduct);
    //  await _context.SaveChangesAsync();
    //  return newProduct;
    //}

    //[UseProjection]
    //public async Task<EntityAddOn> CreateAddOn(
    //    AddOnRequest addOn,
    //    [Service] AppDBContext _context
    //  )
    //{
    //  var product = await _context.Products
    //    .AsNoTracking()
    //    .Where((entity) => entity.Name == addOn.ProductName)
    //    .Select((entity) => new
    //    {
    //      entity.Id,
    //      entity.Name
    //    })
    //    .SingleOrDefaultAsync();

    //  if (product == null)
    //    throw new ArgumentException("ParentNotFound");

    //  var newAddOn = new EntityAddOn
    //  {
    //    Name = addOn.Name,
    //    Price = addOn.Price,
    //    Amount = addOn.Amount,
    //    ProductId = product.Id
    //  };

    //  _context.AddOns.Add(newAddOn);
    //  await _context.SaveChangesAsync();
    //  return newAddOn;
    //}
  }
}