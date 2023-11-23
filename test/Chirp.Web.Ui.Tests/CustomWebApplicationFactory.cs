using Microsoft.AspNetCore.Authentication;

namespace Playwright.App.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _host;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ChirpContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<ChirpContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            var authService = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IAuthenticationService));
            if (authService != null)
            {
                services.Remove(authService);
            }

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });
        });

        builder.UseEnvironment("Development");

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

        _host = builder.Build();
        _host.Start();

        var server = _host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>() ?? throw new InvalidOperationException(
                "No server addresses found.");
        ClientOptions.BaseAddress = addresses.Addresses

            .Select(x => new Uri(x))
            .Last();

        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
    }

    private void EnsureServer()
    {
        if (_host is null)
        {
            // This forces WebApplicationFactory to bootstrap the server
            using var _ = CreateDefaultClient();
        }
    }
}
