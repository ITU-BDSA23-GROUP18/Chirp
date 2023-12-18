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
public class ChirpUiTests : PageTest, IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly string _serverAddress;
    private readonly CustomWebApplicationFactory _fixture;

    private readonly HttpClient _client;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChirpUiTests"/> class.
    /// </summary>
    public ChirpUiTests(CustomWebApplicationFactory fixture)
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

        // The current time is used such that we avoid duplicate cheeps.
        // And make a uniq finger pirnt for this cheep
        for (int i = 0; i < 10; i++)
        {
            var currentTime = DateTime.Now.ToString("HH.mm.ss.ffffff dd.MM.yyyy");
            await page.GetByPlaceholder(new Regex("What's on your heart", RegexOptions.IgnoreCase)).FillAsync(currentTime);
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

            // Await page.GetByText(" Chirp!").ClickAsync();
            await page.GetByPlaceholder(new Regex("What's on your heart", RegexOptions.IgnoreCase)).FillAsync(currentTime);
            await page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();

            // See the cheep in the timeline
            await page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }

        // Go to the user timeline
        await page.GotoAsync(_serverAddress + "TestUser");
        for (int i = 1; i < listOfCheeps.Count; i++)
        {
            await page.GetByText(new Regex(listOfCheeps[i], RegexOptions.IgnoreCase)).ClickAsync();
        }

        // The first should be on the 2nd page
        await page.GetByRole(AriaRole.Link, new() { Name = "2", Exact = true }).ClickAsync();

        await page.GetByText(new Regex(listOfCheeps[0], RegexOptions.IgnoreCase)).ClickAsync();
    }

    [Fact]
    public async Task SeeFollowerTimeLineTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Link, new() { Name = "Following Timeline" }).ClickAsync();

        await page.GetByRole(AriaRole.Heading, new() { Name = "Following Timeline" }).ClickAsync();

        await page.GetByText("You are not following anybody!.").ClickAsync();
    }

    [Fact]
    public async Task FollowUsersTest()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Link, new() { Name = "Following Timeline" }).ClickAsync();

        await page.GetByText("You are not following anybody!.").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();

        await page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

        // Following number does not update in the UI so we have to go to the following page
        await page.GetByRole(AriaRole.Link, new() { Name = "Followers: 1" }).WaitForAsync();

        await page.GotoAsync(_serverAddress + "Helge");

        await page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

        // This is still wrong and shoud be fixed
        await page.GetByRole(AriaRole.Link, new() { Name = "Followers: 1" }).WaitForAsync();

        // Unfollow the users so that the test can be run again
        await page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();

        await page.GotoAsync(_serverAddress + "Helge");

        await page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();
    }

    [Fact]
    public async Task TurnOnDarkModeAndTestEveryPage()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await page.GetByRole(AriaRole.List).Filter(new() { HasText = "Dark mode Enable dark mode" }).Locator("span").ClickAsync();

        // We are just looking to see if the body is there if any errors occur it will be caught by the test and it will fail
        await page.Locator("body").ClickAsync();

        // See if a user timeline is still working
        await page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "Helge");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "FollowingTimeline");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "?page=2");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "TestUser");

        await page.Locator("body").ClickAsync();
    }

    [Fact]
    public async Task Test1dot5TextSize()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await page.Locator("#scale").SelectOptionAsync(new[] { "1.5" });

        await page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "Helge");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "FollowingTimeline");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "?page=2");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "TestUser");

        await page.Locator("body").ClickAsync();
    }

    [Fact]
    public async Task Test2TextSize()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await page.Locator("#scale").SelectOptionAsync(new[] { "2" });

        await page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "Helge");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "FollowingTimeline");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "?page=2");

        await page.Locator("body").ClickAsync();

        await page.GotoAsync(_serverAddress + "TestUser");

        await page.Locator("body").ClickAsync();
    }

    [Theory]
    [InlineData("Good")]
    [InlineData("Ish")]
    [InlineData("Bad")]
    public async Task TestReactionsOnOff(string reaction)
    {
        var page = await _context!.NewPageAsync();
        var cheep = page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Jacqualine Gilcoine" }).Nth(0);
        await page.GotoAsync(_serverAddress);
        switch (reaction)
        {
            case "Good":
                await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 0" }).ClickAsync();
                await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 1" }).ClickAsync();
                break;
            case "Ish":
                await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 0" }).ClickAsync();
                await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 1" }).ClickAsync();
                break;
            case "Bad":
                await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 0" }).ClickAsync();
                await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 1" }).ClickAsync();
                break;
        }
    }

    [Fact]
    public async Task TestReactionNoMoreThanOneReactionPerCheep()
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        // The idea behind this test is that we can only react once per cheep,
        // Therefore it should only be possible to see the change in the button if we click it last.
        var cheep = page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Jacqualine Gilcoine" }).Nth(0);
        await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 1" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 1" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "💩 : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "🕶️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 0" }).ClickAsync();
        await cheep.GetByRole(AriaRole.Button, new() { Name = "❤️ : 1" }).ClickAsync();
    }

    [Theory]
    [InlineData("aaa")]
    [InlineData("bbb")]
    [InlineData("ccc")]
    public async Task TestChangeUsername(string newName)
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        var usernameField = page.Locator("#username");

        await usernameField.ClickAsync();
        await usernameField.FillAsync(newName);
        var changeNameButton = page.Locator("#changeusername");
        await changeNameButton.ClickAsync();

        // Go to home page and cheep
        await page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();

        // Look for the new name
        await page.GetByPlaceholder(new Regex(newName, RegexOptions.IgnoreCase)).ClickAsync();

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        usernameField = page.Locator("#username");

        // Change the name back to TestUser
        await usernameField.ClickAsync();
        await usernameField.FillAsync("TestUser");
        changeNameButton = page.Locator("#changeusername");
        await changeNameButton.ClickAsync();
    }

    [Theory]
    [InlineData("TestUser1@Test.Test")]
    [InlineData("TestUser2@Test.Test")]
    [InlineData("TestUser3@Test.Test")]
    public async Task TestChangeEmail(string newEmail)
    {
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress);

        await page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        var emailField = page.Locator("#email");

        await emailField.ClickAsync();
        await emailField.FillAsync(newEmail);
        var changeEmailButton = page.Locator("#changeemail");
        await changeEmailButton.ClickAsync();

        // Change the email back to TestUser@Test.Test
        await emailField.ClickAsync();
        await emailField.FillAsync("TestUser@Test.Test");
        changeEmailButton = page.Locator("#changeemail");
        await changeEmailButton.ClickAsync();
    }

    /// <summary>
    /// This test is used to see if the number of followers is correct.
    /// We know that the user "Wendell Ballan" follows 3 users by default.
    /// </summary>
    [Fact]
    public async Task SeeAUserThatFollowsOtherUsersTest()
    {
        // Wendell Ballan follows 3 users by default if this is changed this test will fail
        var page = await _context!.NewPageAsync();

        await page.GotoAsync(_serverAddress + "Wendell Ballan");

        await page.GetByRole(AriaRole.Link, new() { Name = "Following: 3" }).ClickAsync();
    }

    /// <summary>
    /// Disposes the browser and context after each test.
    /// </summary>
    public void Dispose()
    {
        // Dispose browser and context here
        _context?.DisposeAsync().GetAwaiter().GetResult();
        _browser?.DisposeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Compairs the 2 urls and see if the are the same
    /// Excetion Thrown if they are not the same.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="expectedUrl"></param>
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
    /// <param name="browser"></param>
    /// <returns></returns>
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
    /// Initializes the browser and context for the tests.
    /// </summary>
    private async Task InitializeBrowserAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            // Disable for debugging, and optionally use slowMo.
            // SlowMo = 400,
            Headless = true, // False if you want to see the browser
        });
        _context = await CreateBrowserContextAsync(_browser);
    }
}
