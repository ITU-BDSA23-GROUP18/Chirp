namespace Chirp.Core.DTOs;

public record CheepDTO(string Author, string Message, string Timestamp, List<ReactionDTO> Reactions);
