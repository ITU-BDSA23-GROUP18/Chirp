using Chirp.core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public class PageNumberingModel : PageModel
{
    public readonly int CheepsPerPage = 32;
    public readonly int NPages;
    public int CurrentPage { get; private set; }

    public PageNumberingModel(IEnumerable<CheepDTO> cheeps)
    {
        CurrentPage = 1;
        
        var nCheeps = cheeps.Count();
        NPages = nCheeps / CheepsPerPage;
        if (nCheeps % CheepsPerPage > 0) NPages++;
    }
    
    public void OnGet()
    {
    }
}
