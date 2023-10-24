using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;

    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository repository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
    }
    
    public ActionResult OnGet(string author, [FromQuery]int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        Cheeps = _repository.GetCheepFromAuthor(new Author {Name = author, Email = ""}, page).Result.ToList(); //TODO: Change to DTO
        return Page();
    }
}
