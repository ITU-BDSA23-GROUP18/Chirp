using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> Cheeps { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    
    public string? ProfilePictureUrl { get; private set; }

    public bool IsDarkMode { get; private set; }
    
    public float FontSizeScale { get; private set; }
    
    public PublicModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        Cheeps = new List<CheepDTO>();
        IsDarkMode = false;
        _repository = repository;
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var nCheeps = await _repository.CountCheeps();
        Pagination = new PaginationModel(nCheeps, page);
        
        if (User.Identity.IsAuthenticated)
        {
            ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity.Name!);
            IsDarkMode = await _authorRepository.IsDarkMode(User.Identity.Name!);
            FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity.Name!);
        }
        
        Cheeps = _repository.GetCheep(page).Result.ToList();
        return Page();
    }
    
    public IActionResult OnPostCheep(string message)
    {
        _repository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }
    
    public async Task<string?> GetProfilePicture(string name)
    {
        return await _authorRepository.GetProfilePicture(name);
    }
}
