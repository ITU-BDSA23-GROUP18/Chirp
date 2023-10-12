using Repositories.DTO;

namespace Repositories;

public class CheepRepository : ICheepRepository, IDisposable
{
    private const int CheepsPerPage = 32;
    private readonly CheepContext _cheepDB;

    public CheepRepository()
    {
        _cheepDB = new CheepContext();
        _cheepDB.InitializeDatabase();
    }

    public void Dispose()
    {
        _cheepDB.Dispose();
    }


    public async Task<IEnumerable<MainCheepDTO>> GetCheep(int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Skip(CheepsPerPage * page)
            .Take(CheepsPerPage)
            .Select(c => 
                new MainCheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()))
            .ToListAsync();
    
    public async Task<IEnumerable<MainCheepDTO>> GetCheepFromAuthor(Author attribute, int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == attribute.Name) //TODO: Change to DTO
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new MainCheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()))
            .ToListAsync();
}
