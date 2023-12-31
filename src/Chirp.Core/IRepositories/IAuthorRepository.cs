﻿using Microsoft.AspNetCore.Http;

namespace Chirp.Core.IRepositories;

/// <summary>
/// The IAuthorRepository interface is used to define the methods that the AuthorRepository class must implement.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Gets the author from the database with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>List of AuthorDTOs, empty if not found.</returns>
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string name);

    /// <summary>
    /// Gets the author from the database with the given email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>List of AuthorDTOs, empty if not found.</returns>
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string email);

    /// <summary>
    /// Gets the authors from the database that are following the given pageUserName.
    /// </summary>
    /// <param name="pageUserName"></param>
    /// <returns>List of AuthorDTOs of the users followers.</returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowers(string pageUserName);

    /// <summary>
    /// Gets the authors from the database that the given pageUserName is following.
    /// </summary>
    /// <param name="pageUserName"></param>
    /// <returns>List of AuthorDTOs of the followers of the user.</returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowing(string pageUserName);

    /// <summary>
    /// Creates an author with the given name and email.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="displayName"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> CreateAuthor(string name, string email, string displayName);

    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName.
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> FollowAuthor(string followName, string currentUserName);

    /// <summary>
    /// Unfollows the author with the given followName from the author with the given currentUserName.
    /// </summary>
    /// <param name="followName"></param>
    /// <param name="currentUserName"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> UnfollowAuthor(string followName, string currentUserName);

    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail.
    /// </summary>
    /// <param name="newEmail"></param>
    /// <param name="currentUserName"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> ChangeEmail(string newEmail, string currentUserName);

    /// <summary>
    /// Deletes the author with the given name.
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="currentUserName"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> ChangeName(string newName, string currentUserName);

    /// <summary>
    /// Deletes the author with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>True, if the method succeeded.</returns>
    public Task<bool> DeleteAuthor(string name);

    public Task UploadProfilePicture(string name, IFormFile image);

    public Task DeleteProfilePicture(string name);

    public Task<string> GetProfilePicture(string name);

    // The 4 methods below are for individual user/"Author" preferences:
    public Task<float> GetFontSizeScale(string name);
    public Task SetFontSizeScale(string name, float fontSizeScale);
    public Task<bool> IsDarkMode(string name);
    public Task SetDarkMode(string name, bool isDarkMode);
    public Task<string> GetDisplayName(string name);
    public Task<bool> EnsureAuthorExists(string name);
}
