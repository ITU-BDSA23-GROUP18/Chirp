using SQLitePCL;
using Testcontainers.MsSql;
using Xunit;
using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Tests;

public class CheepRepositoryTests
{
    private readonly ChirpContext _context;
    private readonly CheepRepository _repository;

    public CheepRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        _context = new ChirpContext(builder.Options);
        _repository = new CheepRepository(_context, false);
    }

    public class CheepDTOComparer : IEqualityComparer<CheepDTO>
    {
        public bool Equals(CheepDTO? x, CheepDTO? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            return x.Author == y.Author && x.Message == y.Message && x.Timestamp == y.Timestamp;
        }

        public int GetHashCode(CheepDTO obj) => obj.GetHashCode();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetCheeps_returns32Cheeps(int page)
    {
        SeedData(_context);

        var cheeps = await _repository.GetCheep(page);

        Assert.Equal(32, cheeps.Count());
    }
    //-------------------------------------------------------------

    [Fact]
    public async void GetCheeps_onFirstPage_returns32FirstCheeps()
    {
        _context.InitializeDatabase(true);

        var cheeps = await _repository.GetCheep(); // page = 1

        var allCheeps = DbInitializer.Cheeps.Select(c => c.ToDTO());

        Assert.Equal(32, cheeps.Count());
        Assert.All(cheeps, c => Assert.Contains(c, allCheeps, new CheepDTOComparer()));
    }

    [Fact]
    public async void GetCheeps_onAPageOutOfRange_returnsEmpty()
    {
        SeedData(_context);

        var cheeps = await _repository.GetCheep(666);

        Assert.Empty(cheeps);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string name, string email)
    {
        _context.InitializeDatabase(true);

        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name);

        var aCheeps = new List<CheepDTO>();
        foreach (var c in DbInitializer.Cheeps.Where(c => c.Author.Name == author.Name).Take(32))
        {
            aCheeps.Add(c.ToDTO());
        }
        Assert.All(cheeps, c => Assert.Contains(c, aCheeps, new CheepDTOComparer()));
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 1)]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 2)]

    public async void GetCheepsFromAuthor_givenAuthorAndPage_returns32Cheeps(string name, string email, int page)
    {
        SeedData(_context);

        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, page);
        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty()
    {
        SeedData(_context);
        var author = new Author
        {
            Name = "OndFisk",
            Email = "rasmus@microsoft.com"
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name);

        Assert.Empty(cheeps);
    }

    [Fact]
    public async void GetCheepsFromAuthor_onAPageOutOfRange_returnsEmpty()
    {
        var author = new Author
        {
            Name = "Helge",
            Email = "ropf@itu.dk"
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, 666);

        Assert.Empty(cheeps);
    }

    //Testing CreateCheep

    [Theory]
    [InlineData("Hello my name is Helge", "Helge")]
    [InlineData("I work at Microsoft", "Rasmus")]
    public void CreateCheep_givenCheepWithAuthor_savesThatCheep(string message, string authorName)
    {
        SeedData(_context);

        var author = _context.Authors.First(a => a.Name == authorName);

        _repository.CreateCheep(message, authorName);

        var cheeps = _context.Cheeps;
        Assert.Contains(cheeps, c => c.Message == message && c.Author == author);
    }

    [Theory]
    [InlineData("I love coding <3", "OndFisk")]
    [InlineData("I can walk non water!", "Jesus")]
    public void CreateCheep_givenCheepWithNonExistingAuthor_CreatesAuthor(string message, string authorName)
    {
        _repository.CreateCheep(message, authorName);
        Assert.Contains(_context.Authors, a => a.Name == authorName);
    }

    [Fact]
    public void ManyNewUsers_CanCreateCheeps_andReadCheep()
    {
        //Use GUID as username since the username must be uniqe
        List<CheepDTO> newCheeps = new Faker<CheepDTO>()
            .CustomInstantiator(f => new CheepDTO(Guid.NewGuid().ToString()[4..], f.Random.Words(), f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"), new List<ReactionDTO>(), "", ""))
            .RuleFor(c => c.Message, (f, c) => f.Random.Words())
            .RuleFor(c => c.Timestamp, (f, c) => f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"))
            .GenerateBetween(50, 100);

        foreach (var cheep in newCheeps)
        {
            _repository.CreateCheep(cheep.Message, cheep.Author);
        }

        Assert.AllAsync(newCheeps, async (c) =>
        {
            var cheeps = await _repository.GetCheepFromAuthor(c.Author);
            Assert.Contains(c, cheeps, new CheepDTOComparer());
        });
    }

    static void SeedData(ChirpContext _context)
    {
        var a1 = new Author() { Name = "Roger Histand", Email = "Roger+Histand@hotmail.com" };
        var a2 = new Author() { Name = "Luanna Muro", Email = "Luanna-Muro@ku.dk" };
        var a3 = new Author() { Name = "Wendell Ballan", Email = "Wendell-Ballan@gmail.com" };
        var a4 = new Author() { Name = "Nathan Sirmon", Email = "Nathan+Sirmon@dtu.dk" };
        var a5 = new Author() { Name = "Quintin Sitts", Email = "Quintin+Sitts@itu.dk" };
        var a6 = new Author() { Name = "Mellie Yost", Email = "Mellie+Yost@ku.dk" };
        var a7 = new Author() { Name = "Malcolm Janski", Email = "Malcolm-Janski@gmail.com" };
        var a8 = new Author() { Name = "Octavio Wagganer", Email = "Octavio.Wagganer@dtu.dk" };
        var a9 = new Author() { Name = "Johnnie Calixto", Email = "Johnnie+Calixto@itu.dk" };
        var a10 = new Author() { Name = "Jacqualine Gilcoine", Email = "Jacqualine.Gilcoine@gmail.com" };
        var a11 = new Author() { Name = "Helge", Email = "ropf@itu.dk" };
        var a12 = new Author() { Name = "Rasmus", Email = "rnie@itu.dk" };

        var Authors = new List<Author>() { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 };

        var c1 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
        var c2 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "And then, as he listened to all that''s left o'' twenty-one people.", TimeStamp = DateTime.Parse("2023-08-01 13:15:21") };
        var c3 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "In various enchanted attitudes, like the Sperm Whale.", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };
        var c4 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Unless we succeed in establishing ourselves in some monomaniac way whatever significance might lurk in them.", TimeStamp = DateTime.Parse("2023-08-01 13:14:34") };
        var c5 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "At last we came back!", TimeStamp = DateTime.Parse("2023-08-01 13:14:35") };
        var c6 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "At first he had only exchanged one trouble for another.", TimeStamp = DateTime.Parse("2023-08-01 13:14:13") };
        var c7 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "In the first watch, and every creditor paid in full.", TimeStamp = DateTime.Parse("2023-08-01 13:16:13") };
        var c8 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.", TimeStamp = DateTime.Parse("2023-08-01 13:14:01") };
        var c9 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The folk on trust in me!", TimeStamp = DateTime.Parse("2023-08-01 13:15:30") };
        var c10 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "It is a damp, drizzly November in my pocket, and switching it backward and forward with a most suspicious aspect.", TimeStamp = DateTime.Parse("2023-08-01 13:13:34") };
        var c11 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I had no difficulty in finding where Sholto lived, and take it and in Canada.", TimeStamp = DateTime.Parse("2023-08-01 13:14:11") };
        var c12 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "What did they take?", TimeStamp = DateTime.Parse("2023-08-01 13:14:44") };
        var c13 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "It struck cold to see you, Mr. White Mason, to our shores a number of young Alec.", TimeStamp = DateTime.Parse("2023-08-01 13:13:23") };
        var c14 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "You are here for at all?", TimeStamp = DateTime.Parse("2023-08-01 13:13:18") };
        var c15 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "My friend took the treasure-box to the window.", TimeStamp = DateTime.Parse("2023-08-01 13:15:17") };
        var c16 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "But ere I could not find it a name that I come from.", TimeStamp = DateTime.Parse("2023-08-01 13:17:18") };
        var c17 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Then Sherlock looked across at the window, candle in his wilful disobedience of the road.", TimeStamp = DateTime.Parse("2023-08-01 13:14:30") };
        var c18 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The message was as well live in this way-- SHERLOCK HOLMES--his limits.", TimeStamp = DateTime.Parse("2023-08-01 13:13:40") };
        var c19 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I commend that fact very carefully in the afternoon.", TimeStamp = DateTime.Parse("2023-08-01 13:13:20") };
        var c20 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "In the card-case is a wonderful old man!", TimeStamp = DateTime.Parse("2023-08-01 13:15:42") };
        var c21 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "But this is his name! said Holmes, shaking his hand.", TimeStamp = DateTime.Parse("2023-08-01 13:13:21") };
        var c22 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "She had turned suddenly, and a lady who has satisfied himself that he has heard it.", TimeStamp = DateTime.Parse("2023-08-01 13:15:51") };
        var c23 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "You were dwelling upon the ground, the sky, the spray that he would be a man''s forefinger dipped in blood.", TimeStamp = DateTime.Parse("2023-08-01 13:13:55") };
        var c24 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Mrs. Straker tells us that his mates thanked God the direful disorders seemed waning.", TimeStamp = DateTime.Parse("2023-08-01 13:14:00") };
        var c25 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I don''t like it, he said, and would have been just a little chat with me.", TimeStamp = DateTime.Parse("2023-08-01 13:13:59") };
        var c26 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "With back to my friend, patience!", TimeStamp = DateTime.Parse("2023-08-01 13:16:58") };
        var c27 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Is there a small outhouse which stands opposite to me, so as to my charge.", TimeStamp = DateTime.Parse("2023-08-01 13:14:38") };
        var c28 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I was too crowded, even on a leaf of my adventures, and had a license for the gallows.", TimeStamp = DateTime.Parse("2023-08-01 13:13:35") };
        var c29 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "A draghound will follow aniseed from here to enter into my heart.", TimeStamp = DateTime.Parse("2023-08-01 13:14:38") };
        var c30 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "That is where the wet and shining eyes.", TimeStamp = DateTime.Parse("2023-08-01 13:13:27") };
        var c31 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "If thou speakest thus to me that it was most piteous, that last journey.", TimeStamp = DateTime.Parse("2023-08-01 13:14:34") };
        var c32 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "My friend, said he.", TimeStamp = DateTime.Parse("2023-08-01 13:13:36") };
        var c33 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "He laid an envelope which was luxurious to the back part of their coming.", TimeStamp = DateTime.Parse("2023-08-01 13:13:58") };
        var c34 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Leave your horses below and nerving itself to concealment.", TimeStamp = DateTime.Parse("2023-08-01 13:16:54") };
        var c35 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Still, there are two brave fellows! Ha, ha!", TimeStamp = DateTime.Parse("2023-08-01 13:13:51") };
        var c36 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Well, Mr. Holmes, but glanced with some confidence, that the bed beside him.", TimeStamp = DateTime.Parse("2023-08-01 13:13:18") };
        var c37 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "But I have quite come to Mackleton with me now for a small figure, sir.", TimeStamp = DateTime.Parse("2023-08-01 13:15:23") };
        var c38 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Every word I say to them ahead, yet with their fists and sticks.", TimeStamp = DateTime.Parse("2023-08-01 13:13:39") };
        var c39 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "A well-fed, plump Huzza Porpoise will yield you about saying, sir?", TimeStamp = DateTime.Parse("2023-08-01 13:13:32") };
        var c40 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Holmes glanced at his busy desk, hurriedly making out his watch, and ever afterwards are missing, Starbuck!", TimeStamp = DateTime.Parse("2023-08-01 13:13:26") };
        var c41 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Like household dogs they came at last come for you.", TimeStamp = DateTime.Parse("2023-08-01 13:14:16") };
        var c42 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "To him it had done a great fish to swallow up the steel head of the cetacea.", TimeStamp = DateTime.Parse("2023-08-01 13:17:10") };
        var c43 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Thence he could towards me.", TimeStamp = DateTime.Parse("2023-08-01 13:13:23") };
        var c44 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "There was still asleep, she slipped noiselessly from the shadow lay upon the one that he was pretty clear now.", TimeStamp = DateTime.Parse("2023-08-01 13:14:14") };
        var c45 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Of course, it instantly occurred to him, whom all thy creativeness mechanical.", TimeStamp = DateTime.Parse("2023-08-01 13:13:25") };
        var c46 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "And you''ll probably find some other English whalers I know nothing of my revolver.", TimeStamp = DateTime.Parse("2023-08-01 13:15:09") };
        var c47 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "His necessities supplied, Derick departed; but he rushed at the end of the previous night.", TimeStamp = DateTime.Parse("2023-08-01 13:13:49") };
        var c48 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "We will leave the metropolis at this point of view you will do good by stealth.", TimeStamp = DateTime.Parse("2023-08-01 13:13:59") };
        var c49 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "One young fellow in much the more intimate acquaintance.", TimeStamp = DateTime.Parse("2023-08-01 13:15:23") };
        var c50 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The shores of the middle of it, and you can imagine, it was probable, from the hall.", TimeStamp = DateTime.Parse("2023-08-01 13:14:10") };
        var c51 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "His bridle is missing, so that a dangerous man to be that they had been employed between 8.30 and the boat to board and lodging.", TimeStamp = DateTime.Parse("2023-08-01 13:16:19") };
        var c52 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The room into which one hopes.", TimeStamp = DateTime.Parse("2023-08-01 13:13:19") };
        var c53 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The area before the fire until he broke at clapping, as at Coxon''s.", TimeStamp = DateTime.Parse("2023-08-01 13:15:10") };
        var c54 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "There he sat; and all he does not use his powers of observation and deduction.", TimeStamp = DateTime.Parse("2023-08-01 13:16:38") };
        var c55 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Mr. Thaddeus Sholto WAS with his methods of work, Mr. Mac.", TimeStamp = DateTime.Parse("2023-08-01 13:15:23") };
        var c56 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "The commissionnaire and his hands to unconditional perdition, in case he was either very long one.", TimeStamp = DateTime.Parse("2023-08-01 13:14:22") };
        var c57 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "See how that murderer could be from any trivial business not connected with her.", TimeStamp = DateTime.Parse("2023-08-01 13:13:21") };
        var c58 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I was asking for your lives!''  _Wharton the Whale Killer_.", TimeStamp = DateTime.Parse("2023-08-01 13:13:35") };
        var c59 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Besides,'' thinks I, ''it was only a simple key?", TimeStamp = DateTime.Parse("2023-08-01 13:13:38") };
        var c60 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I thought that you are bored to death in the other.", TimeStamp = DateTime.Parse("2023-08-01 13:16:13") };
        var c61 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "D''ye see him? cried Ahab, exultingly but on!", TimeStamp = DateTime.Parse("2023-08-01 13:15:13") };
        var c62 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "I think, said he, Holmes, with all hands to stand on!", TimeStamp = DateTime.Parse("2023-08-01 13:14:50") };
        var c63 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "It came from a grove of Scotch firs, and I were strolling on the soft gravel, and finally the dining-room.", TimeStamp = DateTime.Parse("2023-08-01 13:14:04") };
        var c64 = new Cheep() { AuthorId = a10.AuthorId, Author = a10, Message = "Nor can piety itself, at such a pair of as a lobster if he had needed it; but no, it''s like that, does he?", TimeStamp = DateTime.Parse("2023-08-01 13:15:42") };

        var Cheeps = new List<Cheep>() { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20, c21, c22, c23, c24, c25, c26, c27, c28, c29, c30, c31, c32, c33, c34, c35, c36, c37, c38, c39, c40, c41, c42, c43, c44, c45, c46, c47, c48, c49, c50, c51, c52, c53, c54, c55, c56, c57, c58, c59, c60, c61, c62, c63, c64 };

        _context.Authors.AddRange(Authors);
        _context.Cheeps.AddRange(Cheeps);
        _context.SaveChanges();
    }
}

