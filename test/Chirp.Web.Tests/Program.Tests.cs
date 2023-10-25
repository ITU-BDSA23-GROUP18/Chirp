namespace Chirp.Web.Tests;

public class TestAPI : IClassFixture<CusomWebApplicationFactory<Program>>
{
    private readonly CusomWebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public TestAPI(CusomWebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("", "-1")]
    [InlineData("Helge", "1")]
    [InlineData("Rasmus", "OndFisk")]
    public async void CanSeePage1(string endpoint, string page)
    {
        var response = await _client.GetAsync($"/{endpoint}?page={page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain($"There are no cheeps so far.", content);
    }

    [Theory]
    [InlineData("", "9999999")]
    [InlineData("Helge", "30")]
    [InlineData("Rasmus", "40")]
    public async void CanSeeEmptyPage(string endpoint, string page)
    {
        var response = await _client.GetAsync($"/{endpoint}?page={page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains($"There are no cheeps so far.", content);
    }
}
