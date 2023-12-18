using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Razor Page")]

public class FollowingTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IReactionRepository _reactionRepository;
    public static List<CheepDTO> Cheeps { get; private set; } = new();
    public static PaginationModel Pagination { get; private set; } = new(1, 1);
    public string? ProfilePictureUrl { get; private set; }
    public bool IsDarkMode { get; private set; }

    public float FontSizeScale { get; private set; }

    public FollowingTimelineModel(ICheepRepository repository, IAuthorRepository authorRepository, IReactionRepository reactionRepository)
    {
        _repository = repository;
        _authorRepository = authorRepository;
        _reactionRepository = reactionRepository;
        IsDarkMode = false;
    }

    public List<CheepDTO> GetCheeps()
        => Cheeps;

    public PaginationModel GetPagination()
        => Pagination;

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        // If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var myFollowing = await _authorRepository.GetFollowing(User.Identity?.Name!);
        foreach (var author in myFollowing)
        {
            var cheeps = _repository.GetCheepFromAuthor(author.Name, page).Result.ToList();
            Cheeps.AddRange(cheeps);
        }

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity.Name!);
            IsDarkMode = await _authorRepository.IsDarkMode(User.Identity.Name!);
            FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity.Name!);
        }

        var nCheeps = Cheeps.Count;
        Pagination = new PaginationModel(nCheeps, page);

        return Page();
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
}
