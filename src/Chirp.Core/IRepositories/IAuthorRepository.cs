using Microsoft.AspNetCore.Http;

namespace Chirp.Core.IRepositories;

public interface IAuthorRepository
{
    // Get methods
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);

    public Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUserName);

    public Task<IEnumerable<AuthorDTO>> GetFollowing(string pageUserName);

    // Post methods
    public void CreateAuthor(string name, string email);

    public void FollowAuthor(string followName, string currentUserName);

    public void UnfollowAuthor(string followName, string currentUserName);

    public void ChangeEmail(string newEmail, string currentUserName);

    public void deleteAuthor(string name);
    
    public Task UploadProfilePicture(string name, IFormFile image);
    
    public Task DeleteProfilePicture(string name);
    
    public Task<string?> GetProfilePicture(string name);
}
