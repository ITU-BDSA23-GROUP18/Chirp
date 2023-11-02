namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _authorDb;

    public AuthorRepository(ChirpContext authorDb)
    {
        _authorDb = authorDb;
        _authorDb.InitializeDatabase();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name) =>
         await _authorDb.Authors
             .Where(a => a.Name == name)
             .Select(a => 
                 new AuthorDTO(a.Name, a.Email))
             .ToListAsync();

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email) =>
        await _authorDb.Authors
            .Where(a => a.Name == email)
            .Select(a => 
                new AuthorDTO(a.Name, a.Email))
            .ToListAsync();

    public void CreateAuthor(string name, string email)
    {
        var nameCheck = _authorDb.Authors.Any(a => a.Name == name);
        var emailCheck = _authorDb.Authors.Any(a => a.Email == email);
        if (nameCheck)
        {
            throw new ArgumentException($"Username {name} is already used");
        }

        if (emailCheck)
        {
            throw new ArgumentException($"{email} is already used!");
        }
        
        var author = new Author
        {
            AuthorId = Guid.NewGuid(),
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        _authorDb.Authors.Add(author);
        _authorDb.SaveChanges();
    }
}
