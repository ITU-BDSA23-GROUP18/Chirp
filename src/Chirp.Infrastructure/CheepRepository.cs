using System.ComponentModel;
using System.Reflection.Metadata;

namespace Chirp.Infrastructure;

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

    public void CreateCheep(string message, string username)
    {

        Author author;
        
        //check if user exists
        if (!_cheepDb.Authors.Any(a => a.Name == username))
        {
            author = new Author
            {
                AuthorId = Guid.NewGuid(),
                Name = username,
                Email = ""

            };
        }
        else
        {
            author = _cheepDb.Authors.SingleAsync(a => a.Name == username).Result;
        }
        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Author = author,
            Message = message,
            TimeStamp = DateTime.UtcNow
        };
        _cheepDb.Cheeps.Add(cheep);
        _cheepDb.SaveChanges();
    }
}

