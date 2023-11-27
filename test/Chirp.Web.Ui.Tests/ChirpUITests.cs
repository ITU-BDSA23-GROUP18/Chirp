namespace Chirp.Web.Ui.Tests;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Playwright.App.Tests.Infrastructure;
using Xunit;

/// <summary>
/// The UiTest class is used to test the UI of the application.
/// </summary>
public class ChirpUITests : PageTest, IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly string _serverAddress;
    private readonly CustomWebApplicationFactory _fixture;

    private readonly HttpClient _client;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChirpUITests"/> class.
    /// </summary>
    public ChirpUITests(CustomWebApplicationFactory fixture)
    {
        _serverAddress = fixture.ServerAddress;
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true,
        });

        InitializeBrowserAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task OpenPageTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);
    }

    [Fact]
    public async Task CreateCheepTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        // The current time is used such that we avoid duplicate cheeps
        // And make a uniqe finger print for this cheep
        for (int i = 0; i < 10; i++)
        {
            var currentTime = DateTime.Now.ToString("HH.mm.ss.ffffff dd.MM.yyyy");

            await page.GetByPlaceholder("What's on your heart, TestUser?").FillAsync(currentTime);
            await page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();

            // See the cheep in the timeline
            await page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }
    }

    [Fact]
    public async Task GoToNextPageTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);
        for (int i = 2; i < 10; i++)
        {
            await page.GetByRole(AriaRole.Link, new() { Name = i.ToString(), Exact = true }).ClickAsync();
            CheckUrl(page.Url, _serverAddress + "?page=" + i);
        }
    }

    [Fact]
    public async Task Create_Cheeps_and_see_userTimeLine()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        var listOfCheeps = new List<string>();

        // 33 is used because one page can only hold 32 cheeps
        for (int i = 0; i < 33; i++)
        {
            var currentTime = DateTime.Now.ToString("HH.mm.ss.ffffff dd.MM.yyyy");
            listOfCheeps.Add(currentTime);

            await page.GetByPlaceholder("What's on your heart, TestUser?").FillAsync(currentTime);
            await page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();

            // See the cheep in the timeline
            await page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }

        // Go to the user timeline
        await page.GetByText("TestUser's page").ClickAsync();
        for (int i = 1; i < listOfCheeps.Count; i++)
        {
            await page.GetByText(new Regex(listOfCheeps[i], RegexOptions.IgnoreCase)).ClickAsync();
        }

        // The first should be on the 2nd page
        await page.GetByRole(AriaRole.Link, new() { Name = "2", Exact = true }).ClickAsync();

        await page.GetByText(new Regex(listOfCheeps[0], RegexOptions.IgnoreCase)).ClickAsync();
    }

    /// <summary>
    /// Disposes the browser and context after each test.
    /// </summary>
    public void Dispose()
    {
        // Dispose browser and context here
        _context?.DisposeAsync().GetAwaiter().GetResult();
        _browser?.DisposeAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initializes the browser and context for the tests.
    /// </summary>
    private async Task InitializeBrowserAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            // Disable for debugging, and optionally use slowMo.
            //SlowMo = 400,
            Headless = true //false if you want to see the browser
        });
        _context = await CreateBrowserContextAsync(_browser);
    }

    [Fact]
    public async Task OpenPageTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);
    }
    [Fact]
    public async Task CreateCheepTest()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);
        //the current time is used such that we avoid duplicate cheeps 
        //and make a uniq finger pirnt for this cheep
        for (int i = 0; i < 10; i++)
        {
            var currentTime = DateTime.Now.ToString("HH.mm.ss.ffffff dd.MM.yyyy");
            //await page.GetByText(" Chirp!").ClickAsync();
            await Page.GetByPlaceholder("What's on your heart, TestUser?").FillAsync(currentTime);
            await Page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();
            //see the cheep in the timeline
            await Page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }
    }
    [Fact]
    public async Task GoToNextPageTest()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);
        for (int i = 2; i < 10; i++)
        {
            await Page.GetByRole(AriaRole.Link, new() { Name = i.ToString(), Exact = true }).ClickAsync();
            CheckUrl(Page.Url, _serverAddress + "?page=" + i);
        }
    }
    [Fact]
    public async Task Create_Cheeps_and_see_userTimeLine()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        var ListOfCheeps = new List<string>();
        //33 is used because one page can only hold 32 cheeps
        for (int i = 0; i < 33; i++)
        {
            var currentTime = DateTime.Now.ToString("HH.mm.ss.ffffff dd.MM.yyyy");
            ListOfCheeps.Add(currentTime);
            //await page.GetByText(" Chirp!").ClickAsync();
            await Page.GetByPlaceholder("What's on your heart, TestUser?").FillAsync(currentTime);
            await Page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();
            //see the cheep in the timeline
            await Page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }
        //go to the user timeline
        await Page.GotoAsync(_serverAddress + "TestUser");
        for (int i = 1; i < ListOfCheeps.Count; i++)
        {
            await Page.GetByText(new Regex(ListOfCheeps[i], RegexOptions.IgnoreCase)).ClickAsync();
        }
        //the first should be on the 2nd page
        await Page.GetByRole(AriaRole.Link, new() { Name = "2", Exact = true }).ClickAsync();

        await Page.GetByText(new Regex(ListOfCheeps[0], RegexOptions.IgnoreCase)).ClickAsync();
    }
    [Fact]
    public async Task SeeFollowerTimeLineTest()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress );

        await Page.GetByRole(AriaRole.Link, new() { Name = "Following Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Following Timeline" }).ClickAsync();

        await Page.GetByText("You are not following anybody!.").ClickAsync();
    }

    [Fact]
    public async Task FollowUsersTest()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Following Timeline" }).ClickAsync();

        await Page.GetByText("You are not following anybody!.").ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();

        await Page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

        //following number does not update in the UI so we have to go to the following page
        await Page.GetByRole(AriaRole.Link, new() { Name = "Followers: 1" }).WaitForAsync();

        await Page.GotoAsync(_serverAddress + "Helge");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();
        //this is still wrong and shoud be fixed
        await Page.GetByRole(AriaRole.Link, new() { Name = "Followers: 1" }).WaitForAsync();

        //unfollow the users so that the test can be run again
        await Page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();

        await Page.GotoAsync(_serverAddress + "Helge");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();
    }
    
    /// <summary>
    /// This test is used to see if the number of followers is correct.
    /// We know that the user "Wendell Ballan" follows 3 users by default
    /// </summary>
    [Fact]
    public async Task seeAUserThatFollowsOtherUsersTest()
    {
        //Wendell Ballan follows 3 users by default if this is changed this test will fail

        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress + "Wendell Ballan");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Following: 3" }).ClickAsync();
    }

    /// <summary>
    /// Compairs the 2 urls and see if the are the same
    /// Excetion Thrown if they are not the same.
    /// </summary>
    /// <param name="url">Url to test.</param>
    /// <param name="expectedUrl">The expected Url.</param>
    private void CheckUrl(string url, string expectedUrl)
    {
        if (url.Equals(expectedUrl) == false)
        {
            throw new Exception("The page url is not correct expected: " + expectedUrl + " but was: " + url + "");
        }
    }

    /// <summary>
    /// Creates the Browser context for each test.
    /// </summary>
    /// <param name="browser">The browser to create context for.</param>
    /// <returns>The context for the browser.</returns>
    private async Task<IBrowserContext> CreateBrowserContextAsync(IBrowser browser)
    {
        var contextOptions = new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true, // This will ignore HTTPS errors
        };
        var context = await browser.NewContextAsync(contextOptions);

        return context;
    }
    
    /// <summary>
    /// Disposes the browser and context after each test
    /// </summary>
    public void Dispose()
    {
        // Dispose browser and context here
        _context?.DisposeAsync().GetAwaiter().GetResult();
        _browser?.DisposeAsync().GetAwaiter().GetResult();
    }
}
