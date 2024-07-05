using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.Products;
using CRM.Core.Interfaces.Services.BLogicServices.ProductsServices;

namespace CRM.Application.Services.BLogicServices.ProductsServices
{
  public class ProductsComponents(
      IProductsRepository repository
    ) : IProductsComponents
  {
    private readonly IProductsRepository _repository = repository;

    public void ValidateObjectCountForDeletion(params string[] names)
    {
      if (names.Length <= 0)
        throw new CustomException(ErrorTypes.BadRequest, $"Objects not found");

      if (names.Length > 10)
        throw new CustomException(ErrorTypes.BadRequest, $"You can delete up to 10 objects at a time");
    }

    public async Task<List<TModel>> GetRemoveItems<TModel, TEntity>(params string[] names) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName
    {
      var result = new List<TModel>();

      foreach (var name in names)
      {
        var entity = await _repository.FindEntityByNameAsync<TModel, TEntity>(name);
        result.Add(entity);
      }

      return result;
    }
  }
}