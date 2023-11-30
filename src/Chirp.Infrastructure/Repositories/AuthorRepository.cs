
namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _authorDb;

    public AuthorRepository(ChirpContext authorDb, bool seedDatabase = true)
    {
        _authorDb = authorDb;
        _authorDb.InitializeDatabase(seedDatabase);
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name) =>
         await _authorDb.Authors
             .Where(a => a.Name == name)
             .Select(a => a.ToDTO())
             .ToListAsync();

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email) =>
        await _authorDb.Authors
            .Where(a => a.Email == email)
            .Select(a => a.ToDTO())
            .ToListAsync();

    public async Task<bool> CreateAuthor(string name, string email)
    {
        if (_authorDb.Authors.Any(a => a.Name == name))
            throw new ArgumentException($"Username {name} is already used");

        if (_authorDb.Authors.Any(a => a.Email == email))
            throw new ArgumentException($"{"email"} is already used!");

        var author = new Author
        {
            AuthorId = Guid.NewGuid(),
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>(),
            Following = new List<Author>(),
            Followers = new List<Author>()
        };
        _authorDb.Authors.Add(author);
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FollowAuthor(string followName, string currentUserName)
    {
        var authorToFollow = await _authorDb.Authors.SingleAsync(a => a.Name == followName);
        if (authorToFollow == null)
        {
            throw new ArgumentException($"Author to follow does not exist");
        }

        var signedInUser = await _authorDb.Authors.Include(author => author.Following).FirstOrDefaultAsync(a => a.Name == currentUserName);
        if (signedInUser == null)
        {
            throw new ArgumentException($"Current user does not exist");
        }
        signedInUser.Following.Add(authorToFollow);
        authorToFollow.Followers.Add(signedInUser);
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUser)
    {
        var user = await _authorDb.Authors.Include(author => author.Followers).FirstOrDefaultAsync(a => a.Name == pageUser);
        //return list of authors in following
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }
        // return list of authors in following
        var followerList = user.Followers.ToList();
        var followerListDto = new List<AuthorDTO>();
        foreach (var author in followerList)
        {
            var authorDto = new AuthorDTO(author.Name, author.Email);
            followerListDto.Add(authorDto);
        }
        return followerListDto;
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowing(string userName)
    {
        var user = await _authorDb.Authors.Include(author => author.Following).FirstOrDefaultAsync(a => a.Name == userName);
        //return list of authors in following
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }
        // return list of authors in following
        var followingList = user.Following.ToList();
        var followingListDto = new List<AuthorDTO>();
        foreach (var author in followingList)
        {
            var authorDto = new AuthorDTO(author.Name, author.Email);
            followingListDto.Add(authorDto);
        }
        return followingListDto;
    }

    public async Task<bool> UnfollowAuthor(string followName, string currentUserName)
    {
        var followAuthor = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == followName);
        var currentUser = await _authorDb.Authors.Include(author => author.Following)
            .FirstOrDefaultAsync(a => a.Name == currentUserName);
        if (followAuthor == null || currentUser == null)
        {
            throw new ArgumentException($"Author {followName} does not exist");
        }
        currentUser.Following.Remove(followAuthor);
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeEmail(string name, string newEmail){
        var author = _authorDb.Authors.FirstOrDefault(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException();
        }
        if (_authorDb.Authors.Any(a => a.Email == newEmail)){
            throw new ArgumentException($"{"email"} is already used!");
        }
        author.Email = newEmail;
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAuthor(string name){
        var author = _authorDb.Authors.FirstOrDefault(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }
        Console.WriteLine("Deleting author: " + author.Name);
        _authorDb.Authors.Remove(author);
        await _authorDb.SaveChangesAsync();
        return true;
    }
}
