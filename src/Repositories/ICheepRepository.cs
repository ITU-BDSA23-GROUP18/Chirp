namespace Repositories;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 0);
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(Author author, int page = 0);
}
