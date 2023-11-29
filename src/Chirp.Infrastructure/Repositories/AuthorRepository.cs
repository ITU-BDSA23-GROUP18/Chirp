using Microsoft.AspNetCore.Http;

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
            var authorDto = new AuthorDTO(author.Name, author.Email, author.ProfilePictureUrl);
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
            var authorDto = new AuthorDTO(author.Name, author.Email, author.ProfilePictureUrl);
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

    public void ChangeEmail(string name, string newEmail){
        var author = _authorDb.Authors.FirstOrDefault(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException();
        }
        if (_authorDb.Authors.Any(a => a.Email == newEmail)){
            throw new ArgumentException($"{"email"} is already used!");
        }
        author.Email = newEmail;
        _authorDb.SaveChanges();
    }

    public void deleteAuthor(string name){
        var author = _authorDb.Authors.FirstOrDefault(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }
        Console.WriteLine("Deleting author: " + author.Name);
        _authorDb.Authors.Remove(author);
        _authorDb.SaveChanges();
    }
    
    public async Task UploadProfilePicture(string name, IFormFile file)
    {
        var user = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (user == null)
        {
            Console.WriteLine($"User with Id {name} not found.");
            return;
        }

        // Check file size (10MB limit)
        if (file.Length > 10 * 1024 * 1024)
        {
            Console.WriteLine("File size exceeds the limit of 10MB.");
            return;
        }

        // Check file type (jpeg, png, gif)
        var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            Console.WriteLine("Invalid file type. Accepted file types: jpeg, png, gif.");
            return;
        }

        // generate unique URL for the file and save it in the database
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
        var fileUrl = $"/images/{fileName}";
        
        // delete old profile picture if it exists
        if (user.ProfilePictureUrl != "")
        {
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePictureUrl);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }
        
        user.ProfilePictureUrl = fileUrl;
        await _authorDb.SaveChangesAsync();
    }
    
    public async Task DeleteProfilePicture(string name)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }
        author.ProfilePictureUrl = null;
        await _authorDb.SaveChangesAsync();
    }
    
    public async Task<string?> GetProfilePicture(string name)
    {
        var author = GetAuthorByName(name).Result.FirstOrDefault();
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }
        
        var ProfilePictureUrl = author.ProfilePictureUrl;

        if (ProfilePictureUrl == "")
        {
            return "images/defualt_user_pic.png";
        }
        
        return ProfilePictureUrl;
    }
    
}
