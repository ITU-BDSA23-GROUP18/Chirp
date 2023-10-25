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

    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string attribute, int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Where(c => c.Author.Name == attribute)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public void CreateCheep(string message, Guid currentUserId)
    {
        var author = _cheepDb.Authors.FirstOrDefault(a => a.AuthorId == currentUserId);
        if (author == null) throw new NotImplementedException("Link up with create user");

        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            AuthorId = currentUserId,
            Author = author,
            Message = message,
            TimeStamp = DateTime.Now
        };
        _cheepDb.Cheeps.Add(cheep);
        _cheepDb.SaveChanges();
    }
}
