using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    
    public List<CheepViewModel> Cheeps {get; private set;}

    public PublicModel(ICheepService service)
    {
        Cheeps = new List<CheepViewModel>();
        _service = service;
    }
    
    public IActionResult OnGet([FromQuery]int page)
    {
        //If a page query is not given in the url set the page=1
        Cheeps = _service.GetCheeps(page);
        return Page();
    }
}
