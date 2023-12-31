using Microsoft.AspNetCore.Http;

namespace Chirp.Infrastructure.Repositories;

/// <summary>
/// The AuthorRepository class is used to interact with the database and perform CRUD operations on the Author table.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _authorDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorRepository"/> class.
    /// </summary>
    /// <param name="authorDb"></param>
    /// <param name="seedDatabase">If seedDatabase is true, the database will be seeded with data.</param>
    public AuthorRepository(ChirpContext authorDb, bool seedDatabase = true)
    {
        _authorDb = authorDb;
        _authorDb.InitializeDatabase(seedDatabase);
    }

    /// <summary>
    /// Gets the author from the database with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name) =>
         await _authorDb.Authors
             .Where(a => a.Name == name)
             .Select(a => a.ToDTO())
             .ToListAsync();

    /// <summary>
    /// Gets the author from the database with the given email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email) =>
        await _authorDb.Authors
            .Where(a => a.Email == email)
            .Select(a => a.ToDTO())
            .ToListAsync();

    /// <summary>
    /// Creates an author with the given name and email.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="displayName"></param>
    /// <exception cref="ArgumentException">If any of the parameters are in use.</exception>
    public async Task<bool> CreateAuthor(string name, string email, string displayName)
    {
        if (_authorDb.Authors.Any(a => a.Name == name))
            throw new ArgumentException($"Username {name} is already used");

        if (_authorDb.Authors.Any(a => a.Email == email))
            throw new ArgumentException($"{"email"} is already used!");

        if (_authorDb.Authors.Any(a => a.DisplayName == displayName))
            throw new ArgumentException($"{"name"} is already used!");

        var author = new Author
        {
            AuthorId = Guid.NewGuid(),
            Name = name,
            DisplayName = displayName,
            Email = email,
            Cheeps = new List<Cheep>(),
            Following = new List<Author>(),
            Followers = new List<Author>(),
        };
        _authorDb.Authors.Add(author);
        await _authorDb.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName.
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    /// <exception cref="ArgumentException">Author to follow or user does not exist.</exception>
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

    /// <summary>
    /// Gets the authors from the database that are following the given pageUserName.
    /// </summary>
    /// <param name="pageUser"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">User does not exist.</exception>
    public async Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUser)
    {
        var user = await _authorDb.Authors.Include(author => author.Followers).FirstOrDefaultAsync(a => a.Name == pageUser);

        // Return list of authors in following
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }

        // Return list of authors in following
        var followerList = user.Followers.ToList();
        var followerListDto = new List<AuthorDTO>();
        foreach (var author in followerList)
        {
            var authorDto = new AuthorDTO(author.Name, author.Email, author.DisplayName, author.ProfilePictureUrl);
            followerListDto.Add(authorDto);
        }

        return followerListDto;
    }

    /// <summary>
    /// Gets the authors from the database that the given pageUserName is following.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">User does not exist.</exception>
    public async Task<IEnumerable<AuthorDTO>> GetFollowing(string userName)
    {
        var user = await _authorDb.Authors.Include(author => author.Following).FirstOrDefaultAsync(a => a.Name == userName);

        // Return list of authors in following
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }

        // return list of authors in following
        var followingList = user.Following.ToList();
        var followingListDto = new List<AuthorDTO>();
        foreach (var author in followingList)
        {
            var authorDto = new AuthorDTO(author.Name, author.Email, author.DisplayName, author.ProfilePictureUrl);
            followingListDto.Add(authorDto);
        }

        return followingListDto;
    }

    /// <summary>
    /// Unfollows the author with the given followName from the author with the given currentUserName.
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    /// <exception cref="ArgumentException">Author to unfollow does not exist.</exception>
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
        followAuthor.Followers.Remove(currentUser);

        await _authorDb.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newEmail"></param>
    /// <exception cref="ArgumentException">If the author is not found, or the email is in use.</exception>
    public async Task<bool> ChangeEmail(string name, string newEmail)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException();
        }

        if (_authorDb.Authors.Any(a => a.Email == newEmail))
        {
            throw new ArgumentException($"{"email"} is already used!");
        }

        author.Email = newEmail;
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeName(string name, string newName)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException();
        }

        if (_authorDb.Authors.Any(a => a.DisplayName == newName))
        {
            throw new ArgumentException($"{"name"} is already used!");
        }

        author.DisplayName = newName;
        await _authorDb.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes the author with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentException">If the given author is not found.</exception>
    public async Task<bool> DeleteAuthor(string name)
    {
        var author = _authorDb.Authors.FirstOrDefault(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        foreach (var follower in await GetFollowers(author.Name))
        {
            await UnfollowAuthor(follower.Name, author.Name);
        }

        _authorDb.Authors.Remove(author);
        await _authorDb.SaveChangesAsync();
        return true;
    }

    public async Task UploadProfilePicture(string name, IFormFile file)
    {
        var user = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (user == null)
        {
            throw new ArgumentException($"User with Id {name} not found.");
        }

        // Check file size (10MB limit)
        if (file.Length > 10 * 1024 * 1024)
        {
            throw new ArgumentException("File size exceeds the limit of 10MB.");
        }

        // Check file type (jpeg, png, gif)
        var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Invalid file type. Accepted file types: jpeg, png, gif.");
        }

        // Generate unique name for the file
        var fileName = $"{Guid.NewGuid()}{fileExtension}";

        // Build the relative file path within the wwwroot/images directory
        var relativeFilePath = Path.Combine("images", fileName);

        // Combine with the absolute path of the wwwroot folder
        var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFilePath);

        // Remove/delete old profile picture if it exists
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePictureUrl.TrimStart('/'));

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }

        // Save the image on the server with the new file name
        await using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Set the user's profile picture URL
        user.ProfilePictureUrl = $"/{relativeFilePath}";

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

    public async Task<string> GetProfilePicture(string name)
    {
        var author = (await GetAuthorByName(name)).FirstOrDefault();
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        var profilePictureUrl = author.ProfilePictureUrl;

        if (string.IsNullOrEmpty(profilePictureUrl) || profilePictureUrl == "")
        {
            profilePictureUrl = "../../images/default_user_pic.png";
        }

        return profilePictureUrl;
    }

    public async Task SetDarkMode(string name, bool isDarkMode)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        author.IsDarkMode = isDarkMode;
        await _authorDb.SaveChangesAsync();
    }

    public async Task<bool> IsDarkMode(string name)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        return author.IsDarkMode;
    }

    public async Task SetFontSizeScale(string name, float fontSizeScale)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        author.FontSizeScale = fontSizeScale;
        await _authorDb.SaveChangesAsync();
    }

    public async Task<float> GetFontSizeScale(string name)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        return author.FontSizeScale;
    }

    public async Task<string> GetDisplayName(string name)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            throw new ArgumentException($"Author {name} does not exist");
        }

        return author.DisplayName;
    }

    public async Task<bool> EnsureAuthorExists(string name)
    {
        var author = await _authorDb.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (author == null)
        {
            await CreateAuthor(name, name, name);
        }

        return true;
    }
}
