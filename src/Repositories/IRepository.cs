namespace Repositories;

public interface IRepository<T, TDto, in TFilter>
{
    public Task<IEnumerable<TDto>> Get(int page = 0);
    public Task<IEnumerable<TDto>> GetFrom(TFilter attribute, int page = 0);
}
