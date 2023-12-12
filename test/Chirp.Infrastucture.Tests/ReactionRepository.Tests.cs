using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Tests;

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
    [InlineData("reactionString")]
    public async Task TestCreateReaction(string reactionString)
    {
        var cheep = _context.Cheeps.FirstOrDefault();
        _repository.CreateReaction(cheep.CheepId.ToString(), cheep.Author.Name.ToString(), "Good");
        var reaction = _context.Reactions.FirstOrDefault();
        Assert.NotNull(reaction);
        Assert.Equal(cheep.CheepId.ToString() , reaction.CheepId.ToString());
        Assert.Equal(cheep.CheepId.ToString(), reaction.AuthorName);
        Assert.Equal(ReactionType.Good, reaction.ReactionType);
    }
    [Fact]
    public async Task TestGetReaction()
    {

    }

}
