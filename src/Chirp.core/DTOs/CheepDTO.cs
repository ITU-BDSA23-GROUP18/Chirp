namespace Chirp.Core;

public record CheepDTO(string Author, string Message, string Timestamp, List<ReactionDTO> Reactions);
