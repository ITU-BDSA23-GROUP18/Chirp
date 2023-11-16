using Chirp.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.core.DTOs;
using Chirp.Web.Pages.Shared;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> Cheeps { get; set; }
    public PaginationModel? Pagination { get; private set; }

    public List<AuthorDTO> FollowersList { get; set; }

    public List<AuthorDTO?> FollowingList { get; set; }

    public int FollowingCount { get; set; }
    
    public bool IsFollowingAuthor { get; set; }
    
    

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        Cheeps = new List<CheepDTO>();
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowersList = new List<AuthorDTO>();
        FollowingList = new List<AuthorDTO?>();
        FollowingCount = 0;
    }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var nCheeps = await _cheepRepository.CountCheepsFromAuthor(author);
        Pagination = new PaginationModel(nCheeps, page);

        var following = await _authorRepository.GetFollowing(author);
        FollowingCount = following.Count();
        
        var myFollowing = await _authorRepository.GetFollowing(User.Identity?.Name!);
        var pageUser = await _authorRepository.GetAuthorByName(author);
        IsFollowingAuthor = myFollowing.Contains(pageUser.FirstOrDefault());
        Console.WriteLine($"Isfollowing: {IsFollowingAuthor}");
        
        /*var followers = await _authorRepository.GetFollowers(author);
        FollowersCount = followers.Count();*/ 

        Cheeps = _cheepRepository.GetCheepFromAuthor(author, page).Result.ToList();
        return Page();

    }

    public IActionResult OnPostUnfollow(string author)
    {
        try
        {
            _authorRepository.UnfollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage("Public");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToPage("Public");
        }
    }
    
    public IActionResult OnPostFollow(string author)
    {
        Console.WriteLine(author);
        try
        {
            _authorRepository.FollowAuthor(author, User.Identity?.Name!);
            return RedirectToPage("Public");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToPage("Public");
        }
    }

}
