using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IRepository<Cheep, Author> _repository;

    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(IRepository<Cheep, Author> repository)
    {
        _repository = repository;
    }
    
    public ActionResult OnGet(string author, [FromQuery]int page)
    {
        Cheeps = _repository.GetFrom(new Author {Name = author, Email = ""}, page == 0 ? 1 : page).Result; //TODO: Change to DTO
        return Page();
    }
}
