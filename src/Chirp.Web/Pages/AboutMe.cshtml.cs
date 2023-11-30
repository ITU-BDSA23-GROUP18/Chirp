using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query;
namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly ICheepRepository _repository;
    public readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> yourCheeps { get; private set; }
    public List<AuthorDTO> Followers { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    public string Email { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    
    // for individual user/"Author" preferences:
    public bool IsDarkMode { get; private set; }
    public int fontSizeScale { get; private set; }
    
    public AboutMeModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        yourCheeps = new List<CheepDTO>();
        Followers = new List<AuthorDTO>();
        _repository = repository;
        _authorRepository = authorRepository;
    }
    /// <summary>
    /// Gets the cheeps from the author with the given currentUserName
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var Authors = await _authorRepository.GetAuthorByName(User.Identity?.Name!);
        var Author = Authors.FirstOrDefault();
        if (Author != null && Author.Email != User.Identity?.Name!)
        {
            Email = Author.Email;
        }
        else 
        {
            Email = "Email...";
        }
        
        foreach (var author in Authors)
        {
            var Followers = _authorRepository.GetFollowers(author.Name).Result.ToList();
            var cheeps = _repository.GetCheepFromAuthor(author.Name, page).Result.ToList();
            yourCheeps.AddRange(cheeps);
            this.Followers.AddRange(Followers);
        }
        
        var nCheeps = yourCheeps.Count;
        Pagination = new PaginationModel(nCheeps, page);

        ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity?.Name!);
        
        IsDarkMode = await _authorRepository.IsDarkMode(User.Identity?.Name!);
        
        fontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity?.Name!);
        
        return Page();
    }
    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail
    /// </summary>
    /// <param name="newEmail"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostChangeEmail(string newEmail)
    {
        try
        {
            Console.WriteLine(newEmail);
            await _authorRepository.ChangeEmail(User.Identity?.Name!,newEmail );
            return RedirectToPage();
        }
        catch 
        {
            Console.WriteLine(newEmail + " is already taken");
            return RedirectToPage();
        }
    }
    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostDeleteAccount(string authorName)
    {
        try
        {
            Console.WriteLine("the author name is:"+authorName);
            await _authorRepository.DeleteAuthor(authorName);
            //Need to signout the user
            return RedirectToPage("Public");
        }
        catch 
        {
            Console.WriteLine("the author name is:"+authorName);
            Console.WriteLine("Author"+ authorName+"does not exist");
            return RedirectToPage();
        }
    }
    public async Task<IActionResult> OnPostSetDarkMode()
    {
        var isDarkMode = !await _authorRepository.IsDarkMode(User.Identity?.Name!);
        
        await _authorRepository.SetDarkMode(User.Identity?.Name!, isDarkMode);
        return RedirectToPage();
    }
}
