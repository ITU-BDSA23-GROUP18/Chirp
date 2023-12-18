namespace Chirp.Web.Ui.Tests;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Playwright.App.Tests.Infrastructure;
using Xunit;
/// <summary>
/// The UiTest class is used to test the UI of the application
/// </summary>
public class UiTest : PageTest, IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly string _serverAddress;
    private readonly CustomWebApplicationFactory _fixture;

    private readonly HttpClient _client;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    /// <summary>
    /// Constructor for the UiTest
    /// </summary>
    public UiTest(CustomWebApplicationFactory fixture)
    {
        _serverAddress = fixture.ServerAddress;
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });

        InitializeBrowserAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Initializes the browser and context for the tests
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
            await Page.GetByPlaceholder(new Regex("What's on your heart",RegexOptions.IgnoreCase)).FillAsync(currentTime);
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
            await Page.GetByPlaceholder(new Regex("What's on your heart",RegexOptions.IgnoreCase)).FillAsync(currentTime);
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
    [Fact]
    public async Task TurnOnDarkModeAndTestEveryPage()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await Page.GetByRole(AriaRole.List).Filter(new() { HasText = "Dark mode Enable dark mode" }).Locator("span").ClickAsync();
        //we are just looking to see if the body is there if any errors occur it will be caught by the test and it will fail
        await Page.Locator("body").ClickAsync();
        //see if a user timeline is still working
        await Page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "Helge");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "FollowingTimeline");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "?page=2");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "TestUser");

        await Page.Locator("body").ClickAsync();
    }
    [Fact]
    public async Task Test1dot5TextSize()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await Page.Locator("#scale").SelectOptionAsync(new[] { "1.5" });

         await Page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "Helge");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "FollowingTimeline");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "?page=2");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "TestUser");

        await Page.Locator("body").ClickAsync();
    }
    [Fact]
    public async Task Test2TextSize()
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        await Page.Locator("#scale").SelectOptionAsync(new[] { "2" });

         await Page.GotoAsync(_serverAddress + "Jacqualine Gilcoine");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "Helge");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "FollowingTimeline");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "?page=2");

        await Page.Locator("body").ClickAsync();

        await Page.GotoAsync(_serverAddress + "TestUser");

        await Page.Locator("body").ClickAsync();
    }

    [Theory]
    [InlineData("Good")]
    [InlineData("Ish")]
    [InlineData("Bad")]
    public async Task TestReactionsOnOff(string reaction){
        var Page = await _context!.NewPageAsync();
        var filterString = "Jacqualine Gilcoine — 13:17:39 01/08/2023 Starbuck now is what we hear the worst";
        var cheep = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = filterString });
        await Page.GotoAsync(_serverAddress);
        switch (reaction){
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
    public async Task TestReactionNoMoreThanOneReactionPerCheep(){
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);
        //the idea behind this test is that we can only react once per cheep,
        //therefore it should only be possible to see the change in the button if we click it last.
        var filterString = "Jacqualine Gilcoine — 13:17:39 01/08/2023 Starbuck now is what we hear the worst";
        var cheep = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = filterString });
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
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        var usernameField = Page.Locator("#username");
        
        await usernameField.ClickAsync();
        await usernameField.FillAsync(newName);
        var changeNameButton = Page.Locator("#changeusername");
        await changeNameButton.ClickAsync();
        //go to home page and cheep
        await Page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();
        //look for the new name
        await Page.GetByPlaceholder(new Regex(newName, RegexOptions.IgnoreCase)).ClickAsync();

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        usernameField = Page.Locator("#username");

        //change the name back to TestUser
        await usernameField.ClickAsync();
        await usernameField.FillAsync("TestUser");
        changeNameButton = Page.Locator("#changeusername");
        await changeNameButton.ClickAsync();
    }
    
    [Theory]
    [InlineData("TestUser1@Test.Test")]
    [InlineData("TestUser2@Test.Test")]
    [InlineData("TestUser3@Test.Test")] 
    public async Task TestChangeEmail(string newEmail)
    {
        var Page = await _context!.NewPageAsync();

        await Page.GotoAsync(_serverAddress);

        await Page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = " Settings" }).ClickAsync();

        var emailField = Page.Locator("#email");
        
        await emailField.ClickAsync();
        await emailField.FillAsync(newEmail);
        var changeEmailButton = Page.Locator("#changeemail");
        await changeEmailButton.ClickAsync();

        //change the email back to TestUser@Test.Test
        await emailField.ClickAsync();
        await emailField.FillAsync("TestUser@Test.Test");
        changeEmailButton = Page.Locator("#changeemail");
        await changeEmailButton.ClickAsync();
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
    /// Excetion Thrown if they are not the same
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
    /// Creates the Browser context for each test
    /// </summary>
    /// <param name="browser"></param>
    /// <returns></returns>
    private async Task<IBrowserContext> CreateBrowserContextAsync(IBrowser browser)
    {
        var contextOptions = new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true // This will ignore HTTPS errors
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
