using Repositories.DTO;

namespace Repositories;

public interface ICheepRepository
{
    public Task<IEnumerable<MainCheepDTO>> GetCheep(int page = 1);
    public Task<IEnumerable<MainCheepDTO>> GetCheepFromAuthor(Author author, int page = 1);
}
