﻿using Repositories.DTO;

namespace Repositories;

public class CheepRepository : IRepository<Cheep, MainCheepDTO, Author>, IDisposable
{
    private const int CheepsPerPage = 32;
    private readonly CheepContext _cheepDB;

    public CheepRepository()
    {
        _cheepDB = new CheepContext();
    }

    public void Dispose()
    {
        _cheepDB.Dispose();
    }

    public async Task<IEnumerable<MainCheepDTO>> Get(int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Skip(CheepsPerPage * page)
            .Take(CheepsPerPage)
            .Select(c => new MainCheepDTO{
                Author = c.Author.Name,
                Message = c.Text,
                Time = c.TimeStamp.ToString()
            })
            .ToListAsync();

    public async Task<IEnumerable<Cheep>> GetFrom(Author attribute, int page = 0) =>
        await _cheepDB.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author == attribute)
            .Skip(CheepsPerPage * page)
            .Take(CheepsPerPage)
            .Select(c => c)
            .ToListAsync();

    Task<IEnumerable<MainCheepDTO>> IRepository<Cheep, MainCheepDTO, Author>.GetFrom(Author attribute, int page)
    {
        throw new NotImplementedException();
    }
}
