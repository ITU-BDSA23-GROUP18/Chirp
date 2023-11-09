using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public class PaginationModel : PageModel
{
    public readonly int CheepsPerPage = 32;
    public readonly int NPages;
    public readonly int CurrentPage;

    public PaginationModel(int nCheeps, int currentPage)
    {
        CurrentPage = currentPage;
        NPages = nCheeps / CheepsPerPage;
        if (nCheeps % CheepsPerPage > 0) NPages++;
    }
}
