namespace Chirp.Core.IRepositories;

public interface IAuthorRepository
{
    // Get methods
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);
    
    // Post methods
    public void CreateAuthor(string name, string email);
}
