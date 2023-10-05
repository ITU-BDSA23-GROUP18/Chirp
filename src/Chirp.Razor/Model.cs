
class Cheep
{
    public int CheepId { get; set; }
    public Author Author { get; set; }
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
}
class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }

    public string Email { get; set; }
    public List<Cheep> Cheeps { get; set;} = new List<Cheep>();    
}

//Cheep and Author.