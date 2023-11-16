﻿namespace Chirp.Core;

public interface ICheepRepository
{
    //Get cheeps
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 1);
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(String authorName, int page = 1);
    public Task<int> CountCheeps();
    public Task<int> CountCheepsFromAuthor(String authorName);
    
    //Post cheeps
    public void CreateCheep(string message, string username);
}
