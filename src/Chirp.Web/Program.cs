namespace Chirp.Web;
using core;
using Infrastructure;
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
        // Try to get remote connection string
        string? connectionString = builder.Configuration.GetConnectionString("AzureSQLDBConnectionstring");
        if (connectionString == null) throw new Exception("Connection string not found");
        if (!connectionString.Contains("Password")) {
            string? pass = builder.Configuration["Chirp:azuredbkey"];
            if (pass == null) throw new Exception("Local sql password not set");
            connectionString += $"Password={pass};";
        }
        
        builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
        builder.WebHost.UseUrls("https://localhost:7022");
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}
