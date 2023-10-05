namespace Repositories;

public class CheepRepository : IRepository<Cheep, Author>, IDisposable
{
    private readonly CheepContext _cheepDB;

    public CheepRepository()
    {
        _cheepDB = new CheepContext();
    }

    public void Dispose()
    {
        _cheepDB.Dispose();
    }

    public async Task<IEnumerable<Cheep>> Get(int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Skip(32 * page)
            .Take(32)
            .Select(c => c)
            .ToListAsync();

    public async Task<IEnumerable<Cheep>> GetFrom(Author attribute, int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author == attribute)
            .Skip(32 * page)
            .Take(32)
            .Select(c => c)
            .ToListAsync();
}
