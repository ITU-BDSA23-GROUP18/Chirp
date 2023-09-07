namespace SimpleDB;

interface IDatabaseRepository
{
    public IEnuerable<T> Read(int? limit = null);
    public void Store(T record);
}