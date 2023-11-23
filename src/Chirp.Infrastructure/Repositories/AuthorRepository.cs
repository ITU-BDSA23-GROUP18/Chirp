
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

    public void CreateAuthor(string name, string email)
    {
        if (_authorDb.Authors.Any(a => a.Name == name))
            throw new ArgumentException($"Username {name} is already used");

        if (_authorDb.Authors.Any(a => a.Email == email))
            throw new ArgumentException($"{email} is already used!");

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
        _authorDb.SaveChanges();
    }

    public void FollowAuthor(string followName, string currentUserName)
    {
        var authorToFollow = _authorDb.Authors.SingleAsync(a => a.Name == followName);
        if (authorToFollow == null)
        {
            throw new ArgumentException($"Author to follow does not exist");
        }

        var signedInUser = _authorDb.Authors.Include(author => author.Following).FirstOrDefault(a => a.Name == currentUserName);
        if (signedInUser == null)
        {
            throw new ArgumentException($"Current user does not exist");
        }
        signedInUser.Following.Add(authorToFollow.Result);
        authorToFollow.Result.Followers.Add(signedInUser);
        _authorDb.SaveChanges();
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

    public void UnfollowAuthor(string followName, string currentUserName)
    {
        var followAuthor = _authorDb.Authors.FirstOrDefault(a => a.Name == followName);
        var currentUser = _authorDb.Authors.Include(author => author.Following)
            .FirstOrDefault(a => a.Name == currentUserName);
        if (followAuthor == null || currentUser == null)
        {
            throw new ArgumentException($"Author {followName} does not exist");
        }
        currentUser.Following.Remove(followAuthor);
        _authorDb.SaveChanges();
    }
}
