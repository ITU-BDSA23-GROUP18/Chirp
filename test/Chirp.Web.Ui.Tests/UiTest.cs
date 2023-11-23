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
            Headless = false,
            SlowMo = 400,
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
            var currentTime = DateTime.Now.ToString("HH.mm.ss dd.MM.yyyy");
            //await page.GetByText(" Chirp!").ClickAsync();
            await Page.GetByPlaceholder("What's on your heart, mavjdk?").FillAsync(currentTime);
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
            var currentTime = DateTime.Now.ToString("HH.mm.ss dd.MM.yyyy");
            ListOfCheeps.Add(currentTime);
            //await page.GetByText(" Chirp!").ClickAsync();
            await Page.GetByPlaceholder("What's on your heart, mavjdk?").FillAsync(currentTime);
            await Page.GetByRole(AriaRole.Button, new() { Name = " Cheep!" }).ClickAsync();
            //see the cheep in the timeline
            await Page.GetByText(new Regex(currentTime, RegexOptions.IgnoreCase)).ClickAsync();
        }
        //go to the user timeline
        await Page.GetByText("mavjdk's page").ClickAsync();
        for (int i = 1; i < ListOfCheeps.Count; i++)
        {
            await Page.GetByText(new Regex(ListOfCheeps[i], RegexOptions.IgnoreCase)).ClickAsync();
        }
        //the first should be on the 2nd page
        await Page.GetByRole(AriaRole.Link, new() { Name = "2", Exact = true }).ClickAsync();

        await Page.GetByText(new Regex(ListOfCheeps[0], RegexOptions.IgnoreCase)).ClickAsync();
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

        //set cookies because playwright does not support authentication out of the box
        await context.AddCookiesAsync(new[]
        {
                new Cookie
                {
                    Name = ".AspNetCore.Cookies",
                    Value = "CfDJ8Pcsm5J7KqVPhNwOPKWuiAASuV0mlzIbgyOY5cep-hwRa99oN64YiC44QZy90LnxqCqI92PZ0FK5asuVn8OgHXQjMj4iSn25kkgDTT3ixsuu6nmIgmQ84_F-USwChzm8VIxNwip23gWxCVmMmr6wsNewfxXJ_vpSXoQcOTIlauJQj2AHI5vKhvWjMHKglTBLFTrlPag3-mU5wpgVvBUQbCxsEKga79R21sGsNQauDF7M-7SyiJz8N0Q7ClHEZP5WZjibmExEtmpTbgPKtNi6d2qR9980QRiMjh9d13vz6vG8_2DG_miEyW1Hx5DyVZG_puZIFj1JenIQX1GnpDFZ4vnTZ828nNFEKT7maE-pCdKOPPletBq8KeB3zuByiEtu6kYj04uQOEKdu0VKQEVXrFbGGy1C3962w2u4rVsA6hxScacgKi0dyeoMajvJ5-b3K0PYb9vmbnwPyJR6Rvsln3srRz8Om4f88ZhU1F15oAOB1i4NSl2MOkiXUhZZhVeFB6bc1r-xGok8PIGUEqOJBeuur3D3jqV6ZHoBD-DtQGljCJOO1pNcWhsxsKlVHPZteFqulp9kfRk6l_F_O-n6rlMXSq7rN7e81xCIh4pwHRXcloVPfS1wS6kKETzhKCGWeEY2G89jjCVaftAFzKQSDeTMECZvcZSSB95duZ6HPOCOWslTHAzVpkAXbJ2gQdbaJ3VHJnG1HMeXDrS4d7FuZ8TOheOGzmT3wslS2uqQ6vGKioLPtgglPnXRQBL1UtJYat7XRhEnK2s9PzN852iKpPR94AdMv1r9IbBr0tn-JrMlPMP5BgalG82ToFTygEctgRToC41YAUgpW50TPlI75wnP3xNEKAtmndgFH9p21tisKKpU4DfJYaG2BI0C2AOjKTgJf16sB2bBa15V7hu5w3RQrL6Iiv8Y76sYOGQDHtjRP2dmhw5O9rKaaqkW6JFf7zbzxVJ-gLycVz2zAnM3nhMrwTQCWZAEHd9JrZuYEzIujaBpOeX2A_KYq0Whea7ew5Ep8EM4TLapKOpdduKwrSq4ewK__sM-BR5ddW0ZlFv0kSGQ3w8FZIaRMo0L1QtGGcsC4TNzBPGUSZXFqeOpULmoOdJqI8r-6YQQbUojZ1WBV5GP01TTA6_Q3uLLzlALHr2hqgq5WIxfcPEpJKCl5Z4YusznKKOJonieCXvcIH3NSkWs0MkutIhTypJlUIYm5858dQTBAh20fW3EKcqz6AuvplHNP4YXPcqFXnb96t-H6WyYzq-cWop1b4-Yr_yefeJe9tjNnCI4jvTAPnUURUL6s-ax7688SFLJKEmWDt7Ur4vbnI59ldtD97jZlXKeUEU6OuXoaCNE8Hgj76Vxp2W2uPXWVIV6buyL-4bTvJuLoyYLPuz3gZPRcUQCSA1VPcViZTiXsd5GCS2XBTpNEc2y4oO63J5gEF9xHdtHmrk8KobvyuPzyVHsUS6j5Co4p1euSwTR2A8_1afIQPTyocwz2D45_TsKk6zVfjrmnhTPaG5jxzryyEhwMc7I7VYysw",
                    Domain = "localhost:7022",
                    Path = "/",
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteAttribute.None
                }
            });
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
