using AutoMapper;

using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.UserRepositories;

namespace CRM.Data.Repositories.UserRepositories
{
  public class UserRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IUserRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public IQueryable<TModel> GetQueryable<TModel, TEntity>() where TModel : class where TEntity : class
    {
      var entities = _repository.GetQueryable<TEntity>();

      var result = new List<TModel>();

      foreach (var entity in entities)
      {
        var temp = _mapper.Map<TModel>(entity);
        result.Add(temp);
      }

      return result.AsQueryable();
    }
  }
}