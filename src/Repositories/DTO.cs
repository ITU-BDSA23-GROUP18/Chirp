namespace Repositories.DTO
{
    public record MainCheepDTO
    {
       public required string Author {get; set;}
       public required string Message {get; set;}
       public required string Timestamp {get; set;}
    }
}
