    namespace Chirp.Web.Ui.Tests;

    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using Microsoft.Playwright.NUnit;
    using Playwright.App.Tests.Infrastructure;
    using Xunit;

    public class UiTest : PageTest, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _serverAddress;
        private readonly CustomWebApplicationFactory _fixture;

        private readonly HttpClient _client;

        public UiTest(CustomWebApplicationFactory fixture)
        {
            _serverAddress = fixture.ServerAddress;
            _fixture = fixture;
            _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions 
            { 
                AllowAutoRedirect = true, 
                HandleCookies = true 
            });
        }

        [Fact]
        public async Task Navigate_to_counter_ensure_current_counter_increases_on_click()
        {

            //Arrange
            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions{
                Headless = false,
                SlowMo = 600,
            });
            var contextOptions = new BrowserNewContextOptions()
            {
                IgnoreHTTPSErrors = true // This will ignore HTTPS errors
            };
            await using var context = await browser.NewContextAsync(contextOptions);

            var page = await context.NewPageAsync();

            

            await page.GotoAsync(_serverAddress);

            //Act
        }
    }