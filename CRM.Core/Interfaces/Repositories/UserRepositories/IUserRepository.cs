namespace CRM.Core.Interfaces.Repositories.UserRepositories
{
  /// <summary>
  ///   Provides repository methods for managing users.
  /// </summary>
  public interface IUserRepository
  {
    /// <summary>
    /// Gets a queryable collection of entities of the specified type.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>A queryable collection of entities of the specified type.</returns>
    IQueryable<TModel> GetQueryable<TModel, TEntity>() where TModel : class where TEntity : class;
  }
}