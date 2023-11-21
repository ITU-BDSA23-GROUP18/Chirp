﻿namespace Chirp.Infrastructure.Types;

public class Author
{
    public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep>? Cheeps { get; set; }
    public List<Author>? Following { get; set; }
    public List<Author>? Followers { get; set; }
}
