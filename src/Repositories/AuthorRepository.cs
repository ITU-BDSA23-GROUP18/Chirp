using System.Linq.Expressions;
using Microsoft.VisualBasic;

namespace Repositories;

public class AuthorRepository : IAuthorRepository
{
    
    private readonly ChirpContext _AuthorDB;

    public AuthorRepository()
    {
        _AuthorDB = new ChirpContext();
        _AuthorDB.InitializeDatabase();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name) =>
         await _AuthorDB.Authors
             .Where(a => a.Name == name)
             .Select(a => 
                 new AuthorDTO(a.Name, a.Email))
             .ToListAsync();

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email) =>
        await _AuthorDB.Authors
            .Where(a => a.Name == email)
            .Select(a => 
                new AuthorDTO(a.Name, a.Email))
            .ToListAsync();

    public void CreateAuthor(string name, string email)
    {
        var nameCheck = _AuthorDB.Authors.Any(a => a.Name == name);
        var emailCheck = _AuthorDB.Authors.Any(a => a.Email == email);
        if (nameCheck)
        {
            throw new ArgumentException($"Username {name} is already used");
        }

        if (emailCheck)
        {
            throw new ArgumentException($"{email} is already used!");
        }
        
        var author = new Author
        {
            AuthorId = new Guid(),
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        _AuthorDB.Authors.Add(author);
        _AuthorDB.SaveChanges();
    }
}
