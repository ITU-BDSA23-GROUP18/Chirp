namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int limit = 0);
    public void Store(T record);
}
