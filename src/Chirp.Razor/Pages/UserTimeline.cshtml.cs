using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Repositories.DTO;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IRepository<MainCheepDTO, Author> _repository;

    public List<MainCheepDTO> Cheeps { get; set; }

    public UserTimelineModel(IRepository<MainCheepDTO, Author> repository)
    {
        Cheeps = new List<MainCheepDTO>();
        _repository = repository;
    }
    
    public ActionResult OnGet(string author, [FromQuery]int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        Cheeps = _repository.GetFrom(new Author {Name = author, Email = ""}, page).Result.ToList(); //TODO: Change to DTO
        return Page();
    }
}
