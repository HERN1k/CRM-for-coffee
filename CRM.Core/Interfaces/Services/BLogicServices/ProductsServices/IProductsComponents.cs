using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Interfaces.Services.BLogicServices.ProductsServices
{
  /// <summary>
  ///   Provides helper methods for managing products.
  /// </summary>
  public interface IProductsComponents
  {
    /// <summary>
    ///   Validates the count of objects for deletion.
    /// </summary>
    /// <param name="names">The names of the objects to be deleted.</param>
    /// <exception cref="CustomException">Thrown if no objects are found or if more than 10 objects are to be deleted at once.</exception>
    void ValidateObjectCountForDeletion(params string[] names);

    /// <summary>
    ///   Gets the list of items to be removed based on their names.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="names">The names of the entities to be removed.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the list of items to be removed.</returns>
    /// <exception cref="CustomException">Thrown if an entity with the specified name is not found.</exception>
    Task<List<TModel>> GetRemoveItems<TModel, TEntity>(params string[] names) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName;
  }
}