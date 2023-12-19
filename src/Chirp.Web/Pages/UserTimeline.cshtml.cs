using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Razor Page")]
public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IReactionRepository _reactionRepository;
    public static List<CheepDTO> Cheeps { get; set; } = new();
    public static PaginationModel Pagination { get; private set; } = new(1, 1);
    public int FollowingCount { get; set; }
    public int FollowersCount { get; set; }
    public bool IsFollowingAuthor { get; set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? AuthorProfilePictureUrl { get; private set; }
    public bool IsDarkMode { get; private set; }
    public float FontSizeScale { get; private set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _reactionRepository = reactionRepository;
        FollowingCount = 0;
        IsDarkMode = false;
    }

    public List<CheepDTO> GetCheeps()
        => Cheeps;

    public PaginationModel GetPagination()
        => Pagination;

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        // If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var nCheeps = await _cheepRepository.CountCheepsFromAuthor(author);
        Pagination = new PaginationModel(nCheeps, page);

        Cheeps = _cheepRepository.GetCheepFromAuthor(author, page).Result.ToList();

        var following = await _authorRepository.GetFollowing(author);
        FollowingCount = following.Count();

        var followers = await _authorRepository.GetFollowers(author);
        FollowersCount = followers.Count();
        AuthorProfilePictureUrl = await _authorRepository.GetProfilePicture(author);

        if (User.Identity == null || User.Identity.Name == null)
        {
            return Page();
        }
        
        var myFollowing = await _authorRepository.GetFollowing(User.Identity?.Name);
        var pageUser = await _authorRepository.GetAuthorByName(author);
        IsFollowingAuthor = myFollowing.Contains(pageUser.FirstOrDefault());
        
        if (User.Identity.IsAuthenticated)
        {
            ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity.Name!);
            IsDarkMode = await _authorRepository.IsDarkMode(User.Identity.Name!);
            FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity.Name!);
        }

        return Page();
    }

    /// <summary>
    /// Unfollows the author with the given author name.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostUnfollow(string author)
    {
        try
        {
            await _authorRepository.UnfollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage();
        }
        catch
        {
            return RedirectToPage();
        }
    }

    /// <summary>
    /// Follows the author with the given author name.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostFollow(string author)
    {
        try
        {
            await _authorRepository.FollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage();
        }
        catch
        {
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostUploadProfilePicture(IFormFile profilePicture)
    {
        try
        {
            await _authorRepository.UploadProfilePicture(User.Identity?.Name!, profilePicture);
            return RedirectToPage();
        }
        catch (ArgumentException e)
        {
            return RedirectToPage("usertimeline", new { error = e.Message ?? "Invalid File" });
        }
        catch
        {
            return RedirectToPage("usertimeline", new { error = "Error Uploading Profile Picture" });
        }
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
