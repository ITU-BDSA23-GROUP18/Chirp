namespace Chirp.Web;
using Microsoft.EntityFrameworkCore;
using core;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

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
        builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChirpContext")));
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
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

