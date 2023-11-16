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

        Cheeps = _cheepRepository.GetCheepFromAuthor(author, page).Result.ToList();
        return Page();

    }

    public async Task<bool> IsFollowing(string followName, string currentUserName)
    {
        {
            var following = await _authorRepository.GetFollowing(currentUserName);
            var pageUser = await _authorRepository.GetAuthorByName(followName);
            return following.Contains(pageUser.FirstOrDefault());
        }
    }

    public void OnPostUnfollow(string followName, string currentUserName)
    {
        try
        {
            _authorRepository.UnfollowAuthor(followName, currentUserName);
            RedirectToPage("Public");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            RedirectToPage("Public");
        }
    }

    public void OnPostFollow(string followName, string currentUserName)
    {
        Console.WriteLine($"follow name:{followName}, current user name: {currentUserName}");

        try
        {
            _authorRepository.FollowAuthor(followName, currentUserName);
            RedirectToPage("Public");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            RedirectToPage("Public");
        }
    }

}
