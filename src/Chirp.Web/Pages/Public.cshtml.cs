using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    
    public PublicModel(ICheepRepository repository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
    }

    public IActionResult OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var nCheeps = _repository.CountCheeps().Result;
        Pagination = new PaginationModel(nCheeps, page);
        
        Cheeps = _repository.GetCheep(page).Result.ToList();
        return Page();
    }
    
    public IActionResult OnPostCheep([FromQuery] int page, string message)
    {
        _repository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }
}
