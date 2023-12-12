namespace Chirp.Infrastructure.Repositories;

using FluentValidation;

public class CheepRepository : ICheepRepository
{
    private const int CheepsPerPage = 32;
    private readonly ChirpContext _cheepDb;
    
    /// <summary>
    /// Constructor for the CheepRepository class
    /// If seedDatabase is true, the database will be seeded with data
    /// </summary>
    /// <param name="cheepDb"></param>
    /// <param name="seedDatabase"></param>
    public CheepRepository(ChirpContext cheepDb, bool seedDatabase = true)
    {
        _cheepDb = cheepDb;
        _cheepDb.InitializeDatabase(seedDatabase);
    }
    
    /// <summary>
    /// Gets a list of cheeps from the database according to the given page number
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<IEnumerable<CheepDTO>> GetCheep(int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Include(c => c.Reactions)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c => c.ToDTO())
            .ToListAsync();

    /// <summary>
    /// Gets the cheep from the given author
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string authorName, int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Include(c => c.Reactions)
            .OrderByDescending(c => c.TimeStamp)
            .Where(c => c.Author.Name == authorName)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c => c.ToDTO())
            .ToListAsync();
    
    /// <summary>
    /// Counts the number of cheeps
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountCheeps() =>
        await _cheepDb.Cheeps
            .CountAsync();
    
    /// <summary>
    /// Counts the number of cheeps from the given author
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns></returns>
    public async Task<int> CountCheepsFromAuthor(string authorName) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == authorName)
            .CountAsync();
    
    /// <summary>
    /// Creates a cheep with the given message and username
    /// </summary>
    /// <param name="message"></param>
    /// <param name="username"></param>
    /// <exception cref="ValidationException"></exception>
    public void CreateCheep(string message, string username)
    {
        var cheepValidator = new CheepValidator();
        var cheepValidationResult = cheepValidator.Validate(message);
        if (!cheepValidationResult.IsValid)
        {
            throw new ValidationException(cheepValidationResult.Errors);
        }

        Author author;

        //check if user exists
        if (!_cheepDb.Authors.Any(a => a.Name == username))
        {
            author = new Author
            {
                AuthorId = Guid.NewGuid(),
                Name = username,
                Email = Guid.NewGuid().ToString()
            };
        }
        else
        {
            author = _cheepDb.Authors.SingleAsync(a => a.Name == username).Result;
        }
        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            AuthorId = author.AuthorId,
            Author = author,
            Message = message,
            TimeStamp = DateTime.UtcNow
        };
        _cheepDb.Cheeps.Add(cheep);
        _cheepDb.SaveChanges();
    }

    /// <summary>
    /// Class used to validate the cheep to insure that the message is not empty and is less than 160 characters
    /// </summary>
    private class CheepValidator : AbstractValidator<string>
    {
        public CheepValidator()
        {
            RuleFor(s => s).NotEmpty().MaximumLength(160);
        }
    }
}
