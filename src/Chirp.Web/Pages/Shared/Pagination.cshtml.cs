using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Razor Page")]
public class PaginationModel : PageModel
{
    public int CheepsPerPage { get; private set; } = 32;
    public int NPages { get; private set; }
    public int CurrentPage { get; private set; }

    public PaginationModel(int nCheeps, int currentPage)
    {
        CurrentPage = currentPage;
        NPages = nCheeps / CheepsPerPage;
        if (nCheeps % CheepsPerPage > 0) NPages++;
    }
}
