namespace Chirp.Infrastucture;

public class CheepRepository : ICheepRepository
{
    private const int CheepsPerPage = 32;
    private readonly ChirpContext _cheepDb;

    public CheepRepository(ChirpContext cheepDb)
    {
        _cheepDb = cheepDb;
        _cheepDb.InitializeDatabase();
    }

    public async Task<IEnumerable<CheepDTO>> GetCheep(int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string authorName, int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Where(c => c.Author.Name == authorName)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public async Task<int> CountCheeps() =>
        await _cheepDb.Cheeps
            .CountAsync();
    
    public async Task<int> CountCheepsFromAuthor(string authorName) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == authorName)
            .CountAsync();

    public void CreateCheep(string message, string authorName)
    {
        var author = _cheepDb.Authors.FirstOrDefault(a => a.Name == authorName) ?? new Author
        {
            AuthorId = Guid.NewGuid(),
            Email = $"cheep{Guid.NewGuid()}@chirp.dk",
            Name = authorName,
            Cheeps = new List<Cheep>()
        };

        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            AuthorId = author.AuthorId,
            Author = author,
            Message = message,
            TimeStamp = DateTime.Now
        };
        _cheepDb.Cheeps.Add(cheep);
        _cheepDb.SaveChanges();
    }
}
