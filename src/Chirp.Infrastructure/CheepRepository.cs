using FluentValidation;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private const int CheepsPerPage = 32;
    private readonly ChirpContext _cheepDb;

    public CheepRepository(ChirpContext cheepDb)
    {
        _cheepDb = cheepDb;
        _cheepDb.InitializeDatabase();
    }

    public async Task<IEnumerable<CheepDTO>> GetCheep(int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public async Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string authorName, int page = 1) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Where(c => c.Author.Name == authorName)
            .Skip(CheepsPerPage * (page - 1))
            .Take(CheepsPerPage)
            .Select(c =>
                new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString()))
            .ToListAsync();

    public async Task<int> CountCheeps() =>
        await _cheepDb.Cheeps
            .CountAsync();
    
    public async Task<int> CountCheepsFromAuthor(string authorName) =>
        await _cheepDb.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == authorName)
            .CountAsync();
    
    public void CreateCheep(string message, string username)
    {
        var cheepValidator = new CheepValidator();
        var cheepValidationResult = cheepValidator.Validate(new NewCheep{Message = message});
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
                Email = ""

            };
        }
        else
        {
            author = _cheepDb.Authors.SingleAsync(a => a.Name == username).Result;
        }
        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Author = author,
            Message = message,
            TimeStamp = DateTime.UtcNow
        };
        _cheepDb.Cheeps.Add(cheep);
        _cheepDb.SaveChanges();
    }
    
    public class NewCheep
    {
        public required string Message { get; set; }
    }
    
    public class CheepValidator : AbstractValidator<NewCheep>
    {
        public CheepValidator()
        {
            RuleFor(c => c.Message).NotEmpty().MaximumLength(160);
        }
    }
}

