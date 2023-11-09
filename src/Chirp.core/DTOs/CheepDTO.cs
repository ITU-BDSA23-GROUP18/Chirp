namespace Chirp.core;

public record CheepDTO(string Author, string Message, string Timestamp, List<ReactionDTO> Reactions);
