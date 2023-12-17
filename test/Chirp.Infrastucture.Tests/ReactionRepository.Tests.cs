using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Tests;
// Using same repository in 2 test files run in parallel, even if it is a in-memory database, 
// can cause concurrency issues, sometimes. So, we need to run the tests sequentially.
[Collection("Cheep Repository Collection")]
public class ReactionRepository_Tests
{
    private readonly ChirpContext _context;
    private readonly ReactionRepository _repository;

    private readonly CheepRepository _cheep_repository;

    public ReactionRepository_Tests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        _context = new ChirpContext(builder.Options);
        _repository = new ReactionRepository(_context);
        _cheep_repository = new CheepRepository(_context);
    }
    [Theory]
    [InlineData("Good")]
    [InlineData("Ish")]
    [InlineData("Bad")]

    public void TestCreateReaction(string reactionString)
    {
        var cheepList = _context.Cheeps.Where(c => c.Author.Name == "Jacqualine Gilcoine").OrderByDescending(c => c.TimeStamp);
        var cheep = cheepList.FirstOrDefault();
        if (cheep == null) throw new ArgumentException($"No cheep found");
        _repository.CreateReaction(cheep.CheepId.ToString(), cheep.Author.Name.ToString(), reactionString);
        var reactions = _context.Reactions.Where(r => r.CheepId == cheep.CheepId);
        var reaction = reactions.FirstOrDefault();
        Assert.Equal(1, reactions.Count());
        Assert.NotNull(reaction);
        Assert.Equal(cheep.CheepId.ToString() , reaction.CheepId.ToString());
        Assert.Equal(cheep.Author.Name, reaction.AuthorName);
        switch (reactionString)
        {
            case "Good":
                Assert.Equal(ReactionType.Good, reaction.ReactionType);
                break;
            case "Ish":
                Assert.Equal(ReactionType.Ish, reaction.ReactionType);
                break;
            case "Bad":
                Assert.Equal(ReactionType.Bad, reaction.ReactionType);
                break;
            default:
                break;
        }
        //remove the reaction
        _repository.RemoveReaction(cheep.CheepId.ToString(), cheep.Author.Name.ToString());
    }
    [Fact]
    public async Task TestGetReaction()
    {
        var cheepList = await _cheep_repository.GetCheep();
        foreach (var cheep in cheepList)
        {
            var reactionList = cheep.Reactions;
            //should not be null but can be empty if no reactions are present
            Assert.NotNull(reactionList);
        }
    }

    [Theory]
    [InlineData("Good")]
    [InlineData("Ish")]
    [InlineData("Bad")]
    public void TestRemoveReaction(string reactionString){
        var cheepList = _context.Cheeps.Where(c => c.Author.Name == "Jacqualine Gilcoine").OrderByDescending(c => c.TimeStamp);
        var cheep = cheepList.FirstOrDefault();
        if (cheep == null) throw new ArgumentException($"No cheep found");
        _repository.CreateReaction(cheep.CheepId.ToString(), cheep.Author.Name.ToString(), reactionString);
        var reactions = _context.Reactions.Where(r => r.CheepId == cheep.CheepId);
        var reaction = reactions.FirstOrDefault();
        Assert.Equal(1, reactions.Count());
        //remove the reaction
        _repository.RemoveReaction(cheep.CheepId.ToString(), cheep.Author.Name.ToString());
        //check if the reaction is removed
        var reactionList = _context.Reactions.Where(r => r.CheepId == cheep.CheepId).ToList();
        Assert.DoesNotContain(reaction, reactionList);
    }
}
