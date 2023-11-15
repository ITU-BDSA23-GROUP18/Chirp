using Chirp.core;
using Chirp.core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Web.Pages.Shared;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    private static readonly IAuthorRepository? _authorRepository;
    public List<CheepDTO> Cheeps { get; set; }
    public PaginationModel? Pagination { get; private set; }

    public UserTimelineModel(ICheepRepository repository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
    }
    
    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var nCheeps = _repository.CountCheepsFromAuthor(author).Result;
        Pagination = new PaginationModel(nCheeps, page);
        
        Cheeps = _repository.GetCheepFromAuthor(author, page).Result.ToList();
        return Page();
    }

    public static string IsFollowing(string followName, string currentUserName)
    {
        var following = _authorRepository!.GetFollowing(currentUserName).Result;
        var pageUser = _authorRepository.GetAuthorByName(followName).Result.FirstOrDefault();
        return following.Contains(pageUser) ? "Unfollow" : "Follow";
    }
}
