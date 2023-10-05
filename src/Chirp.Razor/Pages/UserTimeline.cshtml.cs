using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;

    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        Cheeps = new List<CheepViewModel>();
        _service = service;
    }
    
    public ActionResult OnGet(string author, [FromQuery]int page)
    {
        Cheeps = _service.GetCheepsFromAuthor(author, page == 0 ? 1 : page);
        return Page();
    }
}
