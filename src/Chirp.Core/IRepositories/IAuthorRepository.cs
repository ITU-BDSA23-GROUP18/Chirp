using Microsoft.AspNetCore.Http;

namespace Chirp.Core.IRepositories;
/// <summary>
/// The IAuthorRepository interface is used to define the methods that the AuthorRepository class must implement
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Gets the author from the database with the given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    /// <summary>
    /// Gets the author from the database with the given email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);
    /// <summary>
    /// Gets the authors from the database that are following the given pageUserName
    /// </summary>
    /// <param name="pageUserName"></param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUserName);
    /// <summary>
    /// Gets the authors from the database that the given pageUserName is following
    /// </summary>
    /// <param name="pageUserName"></param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowing(string pageUserName);
    /// <summary>
    /// Creates an author with the given name and email
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    public Task<bool> CreateAuthor(string name, string email);
    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    public Task<bool> FollowAuthor(string followName, string currentUserName);
    /// <summary>
    /// Unfollows the author with the given followName from the author with the given currentUserName
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    public Task<bool> UnfollowAuthor(string followName, string currentUserName);
    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail
    /// </summary>
    /// <param name="newEmail"></param>
    /// <param name="currentUserName"></param>
     public Task<bool> ChangeEmail(string newEmail, string currentUserName);
    /// <summary>
    /// Deletes the author with the given name
    /// </summary>
    /// <param name="name"></param>
    public Task<bool> DeleteAuthor(string name);
    
    public Task UploadProfilePicture(string name, IFormFile image);
    
    public Task DeleteProfilePicture(string name);
    
    public Task<string?> GetProfilePicture(string name);
}
