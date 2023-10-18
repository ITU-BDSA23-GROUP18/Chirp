namespace Repositories;

public interface ICheepRepository
{
    //Get cheeps
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 0);
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(Author author, int page = 0);
    
    //Post cheeps
    public void CreateCheep(string message, Guid currentUserID);
}
