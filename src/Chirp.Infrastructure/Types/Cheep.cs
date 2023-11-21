﻿namespace Chirp.Infrastructure.Types;

public class Cheep
{
    public Guid CheepId { get; set; }
    public required Guid AuthorId { get; set; }
    public required Author Author { get; set; }
    public required string Message { get; set; }
    public DateTime TimeStamp { get; set; }
    public List<Reaction> Reactions { get; set; } = new();
}