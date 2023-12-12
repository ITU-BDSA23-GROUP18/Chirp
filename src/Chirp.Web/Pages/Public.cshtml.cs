using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IReactionRepository _reactionRepository;
    private static List<CheepDTO> Cheeps { get; set; } = new();
    public static PaginationModel Pagination { get; private set; } = new (1, 1);
    public string? ProfilePictureUrl { get; private set; }

    public bool IsDarkMode { get; private set; }
    
    public float FontSizeScale { get; private set; }
    
    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IReactionRepository reactionRepository)
    {
        IsDarkMode = false;   
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<IActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var nCheeps = _cheepRepository.CountCheeps().Result;
        Pagination = new PaginationModel(nCheeps, page);
        
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity.Name!);
            IsDarkMode = await _authorRepository.IsDarkMode(User.Identity.Name!);
            FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity.Name!);
        }

        Cheeps = _cheepRepository.GetCheep(page).Result.ToList();
        
        return Page();
    }
    
    public List<CheepDTO> GetCheeps()
    {
        return Cheeps;
    }

    public PaginationModel GetPagination()
    {
        return Pagination;
    }
    
    public IActionResult OnPostCheep(string message)
    {
        _cheepRepository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }

    public void OnPostChangeReaction(string cheepId, string reactionType)
    {
        var author = User.Identity?.Name!;
        if (!(User.Identity?.IsAuthenticated ?? false) || author == "") return;

        var cheepReactions = Cheeps.First(c => c.CheepId == cheepId).Reactions;
        if (cheepReactions.Any(r => r.Author == author))
        {
            var prevReaction = cheepReactions.First(r => r.Author == author);
            cheepReactions.Remove(prevReaction);
            _reactionRepository.RemoveReaction(cheepId, author);
            if (prevReaction.ReactionType == reactionType) return;
        }
        
        cheepReactions.Add(new ReactionDTO(cheepId, author, reactionType));
        _reactionRepository.CreateReaction(cheepId, author, reactionType);
    }
    
    public async Task<string?> GetProfilePicture(string name)
    {
        return await _authorRepository.GetProfilePicture(name);
    }
}
