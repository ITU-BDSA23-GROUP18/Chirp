using Chirp.core.DTOs;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _authorDb;

    public AuthorRepository(ChirpDbContext authorDb)
    {
        _authorDb = authorDb;
        _authorDb.InitializeDatabase();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name) =>
         await _authorDb.Authors!
             .Where(a => a.Name == name)
             .Select(a => 
                 new AuthorDTO(a.Name, a.Email))
             .ToListAsync();

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email) =>
        await _authorDb.Authors!
            .Where(a => a.Name == email)
            .Select(a => 
                new AuthorDTO(a.Name, a.Email))
            .ToListAsync();

    public void CreateAuthor(string name, string email)
    {
        var nameCheck =  _authorDb.Authors!.Any(a => a.Name == name);
        var emailCheck = _authorDb.Authors!.Any(a => a.Email == email);
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
        _authorDb.Authors!.Add(author);
        _authorDb.SaveChanges();
    }

    public void FollowAuthor(string followName, string currentUserName)
    {
        var followAuthor = _authorDb.Authors!.FirstOrDefault(a => a.Name == followName);
        var currentUser = _authorDb.Authors!.Include(author => author.Following!).FirstOrDefault(a => a.Name == currentUserName);
        if (followAuthor == null || currentUser == null)
        {
            throw new ArgumentException($"Author {followName} does not exist");
        }
        
        currentUser.Following!.Add(followAuthor);
        _authorDb.SaveChanges();
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowers(string currentUserName)
    {
        var followers = await _authorDb.Authors!
            .Where(a => a.Following!.Any(f => f.Name == currentUserName))
            .Select(a => 
                new AuthorDTO(a.Name, a.Email))
            .ToListAsync();
        return followers;
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowing(string currentUserName)
    {
        var following = await _authorDb.Authors!
            .Where(a => a.Following!.Any(f => f.Name == currentUserName))
            .Select(a => 
                new AuthorDTO(a.Name, a.Email))
            .ToListAsync();
        return following;
    }

    public void UnfollowAuthor(string followName, string currentUserName)
    {
        var followAuthor = _authorDb.Authors!.FirstOrDefault(a => a.Name == followName);
        var currentUser = _authorDb.Authors!.Include(author => author.Following!)
            .FirstOrDefault(a => a.Name == currentUserName);
        if (followAuthor == null || currentUser == null)
        {
            throw new ArgumentException($"Author {followName} does not exist");
        }
        
        currentUser.Following!.Remove(followAuthor);
        _authorDb.SaveChanges();
    }
}
