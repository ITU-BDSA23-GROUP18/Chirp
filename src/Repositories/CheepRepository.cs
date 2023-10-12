namespace Repositories;

public class CheepRepository : ICheepRepository, IDisposable
{
    private const int CheepsPerPage = 32;
    private readonly ChirpContext _cheepDB;

    public CheepRepository()
    {
        _cheepDB = new ChirpContext();
        _cheepDB.InitializeDatabase();
    }

    public void Dispose()
    {
        _cheepDB.Dispose();
    }


    public async Task<IEnumerable<CheepDTO>> GetCheep(int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Skip(CheepsPerPage * page)
            .Take(CheepsPerPage)
            .Select(c => 
                new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()))
            .ToListAsync();
    
    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(Author attribute, int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == attribute.Name) //TODO: Change to DTO
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()))
            .ToListAsync();
}
