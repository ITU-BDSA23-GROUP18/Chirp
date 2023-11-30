using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> Cheeps { get; set; }
    public PaginationModel? Pagination { get; private set; }

    public int FollowingCount { get; set; }
    
    public int FollowersCount { get; set; }
    
    public bool IsFollowingAuthor { get; set; }
    
    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        Cheeps = new List<CheepDTO>();
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowingCount = 0;
    }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var nCheeps = await _cheepRepository.CountCheepsFromAuthor(author);
        Pagination = new PaginationModel(nCheeps, page);

        Cheeps = _cheepRepository.GetCheepFromAuthor(author, page).Result.ToList();

        var following = await _authorRepository.GetFollowing(author);
        FollowingCount = following.Count();
            
        var followers = await _authorRepository.GetFollowers(author);
        FollowersCount = followers.Count();
        
        if (User.Identity == null || User.Identity.Name == null) {
            return Page();
        }
        
        var myFollowing = await _authorRepository.GetFollowing(User.Identity.Name);
        var pageUser = await _authorRepository.GetAuthorByName(author);
        IsFollowingAuthor = myFollowing.Contains(pageUser.FirstOrDefault());
        
        return Page();
    }
    /// <summary>
    /// Unfollows the author with the given author name
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public IActionResult OnPostUnfollow(string author)
    {
        try
        {
            _authorRepository.UnfollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage();
        }
        catch 
        {
            return RedirectToPage();
        }
    }
    /// <summary>
    /// Follows the author with the given author name
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public IActionResult OnPostFollow(string author)
    {
        try
        {
            _authorRepository.FollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage();
        }
        catch 
        {
            return RedirectToPage();
        }
    }
}
