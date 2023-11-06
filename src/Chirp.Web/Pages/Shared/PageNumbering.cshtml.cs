using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public class PageNumberingModel : PageModel
{
    public readonly int CheepsPerPage = 32;
    public readonly int NPages;
    public int CurrentPage { get; set; }

    public PageNumberingModel(int nCheeps, int currentPage)
    {
        CurrentPage = currentPage;
        NPages = nCheeps / CheepsPerPage;
        if (nCheeps % CheepsPerPage > 0) NPages++;
    }
}
