﻿namespace Chirp.core;

public interface IAuthorRepository
{
    // Getter methods
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);

    public Task<IEnumerable<AuthorDTO>> GetFollowers(string name);

    public Task<IEnumerable<AuthorDTO>> GetFollowing(string name);
    
    // Post methods
    public void CreateAuthor(string name, string email);
    
    public void FollowAuthor(string followName, string currentUserName);
    
    public void UnfollowAuthor(string followName, string currentUserName);
}
