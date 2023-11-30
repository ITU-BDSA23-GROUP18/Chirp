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
    
    public AboutMeModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        yourCheeps = new List<CheepDTO>();
        Followers = new List<AuthorDTO>();
        _repository = repository;
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var Author = await _authorRepository.GetAuthorByName(User.Identity?.Name!);
        if (Author.FirstOrDefault().Email != User.Identity?.Name!)
        {
            Email = Author.FirstOrDefault().Email;
        }
        else 
        {
            Email = "Email...";
        }
        
        foreach (var author in Author)
        {
            var Followers = _authorRepository.GetFollowers(author.Name).Result.ToList();
            var cheeps = _repository.GetCheepFromAuthor(author.Name, page).Result.ToList();
            yourCheeps.AddRange(cheeps);
            this.Followers.AddRange(Followers);
        }
        
        var nCheeps = yourCheeps.Count;
        Pagination = new PaginationModel(nCheeps, page);

        ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity?.Name!);
        
        return Page();
    }
     public IActionResult OnPostChangeEmail(string newEmail)
    {
        try
        {
            Console.WriteLine(newEmail);
            _authorRepository.ChangeEmail(User.Identity?.Name!,newEmail );
            return RedirectToPage();
        }
        catch 
        {
            Console.WriteLine(newEmail + " is already taken");
            return RedirectToPage();
        }
    }
    public async Task<ActionResult> OnPostDeleteAccount(string authorName)
    {
        try
        {
            Console.WriteLine("the author name is:"+authorName);
            _authorRepository.deleteAuthor(authorName);
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
    
    public async Task<IActionResult> OnPostUploadProfilePicture(IFormFile profilePicture)
    {
        Console.WriteLine(profilePicture.FileName);
        try
        {
            await _authorRepository.UploadProfilePicture(User.Identity?.Name!, profilePicture);
            return RedirectToPage();
        }
        catch 
        {
            return RedirectToPage();
        }
    }
}
