using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Chirp.Razor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        var dbPath = Path.Combine(Path.GetTempPath(),"Chirp.db");
        builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlite($"Data Source={dbPath}"));
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        
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

        app.MapRazorPages();

        app.Run();
    }
}

