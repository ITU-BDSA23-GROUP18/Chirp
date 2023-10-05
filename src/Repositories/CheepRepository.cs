namespace Repositories;

public class CheepRepository : IRepository<Cheep, Author>
{
    private readonly DBFacade _cheepDB;

    public CheepRepository(DBFacade cheepDB)
    {
        _cheepDB = cheepDB;
    }

    public IEnumerable<Cheep> Get() =>
        from c in _cheepDB.GetCheeps(0, 32)
        select new Cheep(new Author(c.Author, ""), c.Message, DateTime.Parse(c.Timestamp));

    public IEnumerable<Cheep> GetFrom(Author attribute) =>
        from c in _cheepDB.GetCheeps(0, 32)
        where c.Author == attribute.Name
        select new Cheep(new Author(c.Author, ""), c.Message, DateTime.Parse(c.Timestamp));
}
