using CRM.Application.GraphQl.Dto;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.BLogicRepositories.Products
{
  /// <summary>
  /// Repository class for handling operations related to products, categories, and add-ons.
  /// </summary>
  public interface IProductsRepository
  {
    /// <summary>
    /// Retrieves a queryable collection of entities and maps them to models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An IQueryable collection of models.</returns>
    IQueryable<TModel> GetQueryable<TModel, TEntity>() where TModel : class where TEntity : class;

    /// <summary>
    /// Retrieves an enumerable collection of entities and maps them to models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An IEnumerable collection of models.</returns>
    Task<IEnumerable<TModel>> GetEnumerable<TModel, TEntity>() where TModel : class where TEntity : class;

    /// <summary>
    /// Removes multiple entities mapped from the provided models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="removeItems">The list of models to remove.</param>
    Task RemoveManyAsync<TModel, TEntity>(List<TModel> removeItems) where TModel : class where TEntity : class;

    /// <summary>
    /// Ensures that an entity with the specified name does not already exist.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="name">The name to check for uniqueness.</param>
    /// <exception cref="CustomException">Thrown if the name is already in use.</exception>
    Task EnsureUniqueName<T>(string name) where T : class, IEntityWithName;

    /// <summary>
    /// Finds a parent entity by name and maps it to a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="parentName">The name of the parent entity.</param>
    /// <returns>The mapped parent model.</returns>
    /// <exception cref="CustomException">Thrown if the parent entity is not found.</exception>
    Task<TModel> GetParentOrThrowAsync<TModel, TEntity>(string parentName) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName;

    /// <summary>
    /// Saves a new product category based on the provided request and maps it to a model.
    /// </summary>
    /// <param name="request">The request containing product category data.</param>
    /// <returns>The saved product category model.</returns>
    Task<ProductCategory> SaveNewItem(ProductCategoryRequest request);

    /// <summary>
    /// Saves a new product based on the provided parent and request, and maps it to a model.
    /// </summary>
    /// <param name="parent">The parent product category.</param>
    /// <param name="request">The request containing product data.</param>
    /// <returns>The saved product model.</returns>
    Task<Product> SaveNewItem(ProductCategory parent, ProductRequest request);

    /// <summary>
    /// Saves a new add-on based on the provided parent and request, and maps it to a model.
    /// </summary>
    /// <param name="parent">The parent product.</param>
    /// <param name="request">The request containing add-on data.</param>
    /// <returns>The saved add-on model.</returns>
    Task<AddOn> SaveNewItem(Product parent, AddOnRequest request);

    /// <summary>
    /// Finds an entity by name and maps it to a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="name">The name of the entity.</param>
    /// <returns>The mapped model.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the name is null or empty.</exception>
    /// <exception cref="CustomException">Thrown if the entity is not found.</exception>
    Task<TModel> FindEntityByNameAsync<TModel, TEntity>(string name) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName;
  }
}