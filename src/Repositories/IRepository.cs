namespace Repositories;

public interface IRepository<out T, in TFilter>
{
    public IEnumerable<T> Get();
    public IEnumerable<T> GetFrom(TFilter attribute);
}
