namespace Repositories;

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
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c => 
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();
    
    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(Author attribute, int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == attribute.Name)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public void CreateCheep(string message, String authorName)
    {
        var author = _cheepDb.Authors.FirstOrDefault(a => a.Name == authorName) ?? new Author
        {
            AuthorId = new Guid(),
            Email = $"cheep{new Guid()}@chirp.dk",
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
