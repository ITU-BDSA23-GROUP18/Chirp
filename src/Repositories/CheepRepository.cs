namespace Repositories;

public class CheepRepository : ICheepRepository
{
    private const int CheepsPerPage = 32;
    private readonly ChirpContext _cheepDB;

    public CheepRepository(ChirpContext cheepDb)
    {
        _cheepDB = cheepDb;
        _cheepDB.InitializeDatabase();
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
            .Where(c => c.Author.Name == attribute.Name)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()))
            .ToListAsync();

    public void CreateCheep(string message, Guid currentAuthorID)
    {
        var currentAuthor = _cheepDB.Authors.Any(a => a.AuthorId == currentAuthorID);

        if (!currentAuthor) throw new NotImplementedException("Link up with create user");
        var author = _cheepDB.Authors.Find(currentAuthorID); 
        var cheep = new Cheep
        {
            CheepId = new Guid(),
            AuthorId = currentAuthorID,
            Author = author,
            Text = message,
            TimeStamp = DateTime.Now
        };
        _cheepDB.Cheeps.Add(cheep);
        _cheepDB.SaveChanges();
    }
}
