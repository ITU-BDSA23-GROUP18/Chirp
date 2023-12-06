namespace Chirp.Core.DTOs;
/// <summary>
/// The ReactionDTO class is used to transfer data from the ReactionRepository class to the UI
/// </summary>
/// <param name="CheepId"></param>
/// <param name="Author"></param>
/// <param name="ReactionType"></param>
public record ReactionDTO(string CheepId, string Author, string ReactionType);
