namespace Repositories.DTO
{
    public interface CheepDTO{}

    public interface AuthorDTO{}
    public record MainCheepDTO : CheepDTO
    {
       public required string Author {get; set;}
       public required string Message {get; set;}
       public required string Time {get; set;}
    }
}
