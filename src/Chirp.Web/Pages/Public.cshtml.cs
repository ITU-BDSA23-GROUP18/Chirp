using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IReactionRepository _reactionRepository;
    public List<CheepDTO> Cheeps { get; private set; } = new();
    public PaginationModel? Pagination { get; private set; }
    
    public PublicModel(ICheepRepository cheepRepository, IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _reactionRepository = reactionRepository;
    }

    public IActionResult OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var nCheeps = _cheepRepository.CountCheeps().Result;
        Pagination = new PaginationModel(nCheeps, page);
        
        Cheeps = _cheepRepository.GetCheep(page).Result.ToList();
        return Page();
    }
    
    public IActionResult OnPostCheep(string message)
    {
        _cheepRepository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }

    public IActionResult OnPostChangeReaction(string cheepString, string reactionType)
    {
        
        var cheep = JsonConvert.DeserializeObject<CheepDTO>(cheepString);
        var author = User.Identity?.Name!;
        if (!(User.Identity?.IsAuthenticated ?? false) || author == "") return RedirectToPage("Public");
        
        if (cheep.Reactions.Any(r => r.Author == author))
        {
            _reactionRepository.RemoveReaction(cheep.CheepId, author);
        }
        _reactionRepository.CreateReaction(cheep.CheepId, author, reactionType);
        return RedirectToPage("Public");
    }
}
