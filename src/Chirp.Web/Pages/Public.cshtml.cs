using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    
    public List<CheepDTO> Cheeps {get; private set;}

    public PublicModel(ICheepRepository repository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
    }
    
    public IActionResult OnGet([FromQuery]int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        Cheeps =  _repository.GetCheep(page).Result.ToList();
        return Page();
    }
}
