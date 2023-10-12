namespace Repositories;

public interface IAuthorRepository
{
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);
}
