namespace Repositories;

public interface IRepository<T, in TFilter>
{
    public Task<IEnumerable<T>> Get(int page = 0);
    public Task<IEnumerable<T>> GetFrom(TFilter attribute, int page = 0);
}
