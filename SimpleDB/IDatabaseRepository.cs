namespace SimpleDB;

interface IDatabaseRepository<T>
{
    public IEnuerable<T> Read(int? limit = null);
    public void Store(T record);
}