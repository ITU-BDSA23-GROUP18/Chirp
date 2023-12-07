namespace Chirp.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
        builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

        var dbPath = Path.Combine(Path.GetTempPath(), "Chirp.db");
        builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlite($"Data Source={dbPath}"));

        // Try to get remote connection string
        /* string? connectionString = builder.Configuration.GetConnectionString("AzureSQLDBConnectionstring");
         if (connectionString == null) throw new Exception("Connection string not found");
         if (!connectionString.Contains("Password")) {
             string? pass = builder.Configuration["Chirp:azuredbkey"];
             if (pass == null) {
                 Console.WriteLine("Local sql password not set and was not overriden by remote!");
             } else {
                 Console.WriteLine(pass);
                 connectionString += $"Password={pass};";
             }
         }/*
  /*
         builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlServer(connectionString));
 */
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<IReactionRepository, ReactionRepository>();

        // add user on signin if they do not exists
        builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = async context =>
            {
                var authorRepository = context.HttpContext.RequestServices.GetRequiredService<IAuthorRepository>();
                if (context.Principal == null) return;
                var authorName = context.Principal.Identity?.Name;
                if (authorName == null) return;
                var author = await authorRepository.GetAuthorByName(authorName);
                if (!author.Any())
                {
                    await authorRepository.CreateAuthor(authorName, authorName, authorName);
                }
            };
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            /*if (!connectionString.Contains("Password")) {
                string passwordFile = "connectionString.txt";
                if (!File.Exists(passwordFile)) {
                    Console.WriteLine("---- No password for sql server, create a file called connectionString.txt with the password in it. ----");
                }
                using var sr = new StreamReader(passwordFile);
                // Read the stream as a string, and write the string to the console.
                connectionString += $"Password={sr.ReadToEnd()};";
            } */

            app.UseExceptionHandler("/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            builder.WebHost.UseUrls("https://localhost:7022");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();
        app.MapControllers();

        app.UseStaticFiles();

        app.Run();
    }
}
