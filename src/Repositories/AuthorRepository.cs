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
}
