using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Repositories.DTO;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IRepository<Cheep, MainCheepDTO, Author> _repository;

    public List<MainCheepDTO> Cheeps { get; set; }

    public UserTimelineModel(IRepository<Cheep, MainCheepDTO, Author> repository)
    {
        _repository = repository;
    }
    
    public ActionResult OnGet(string author, [FromQuery]int page)
    {
        Cheeps = _repository.GetFrom(new Author {Name = author, Email = ""}, page == 0 ? 1 : page).Result.ToList(); //TODO: Change to DTO
        return Page();
    }
}
