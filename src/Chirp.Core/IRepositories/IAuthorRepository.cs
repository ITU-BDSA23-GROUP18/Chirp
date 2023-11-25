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

    public void ChangeUsername(string newName, string currentUserName);

    public void ChangeEmail(string newEmail, string currentUserName);

    public void deleteAuthor(string name);
}
