namespace Chirp.Core.IRepositories;

public interface IAuthorRepository
{
    // Get methods
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);

    public Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUserName);

    public Task<IEnumerable<AuthorDTO>> GetFollowing(string pageUserName);

    // Post methods
    public Task<bool> CreateAuthor(string name, string email);

    public Task<bool> FollowAuthor(string followName, string currentUserName);

    public Task<bool> UnfollowAuthor(string followName, string currentUserName);

    public Task<bool> ChangeEmail(string newEmail, string currentUserName);

    public Task<bool> DeleteAuthor(string name);
}
