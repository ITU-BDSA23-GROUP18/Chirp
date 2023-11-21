using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Chirp.Web.Pages;

public class FollowingModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    
    public List<AuthorDTO> FollowersList { get; set; }
    public List<AuthorDTO> FollowingList { get; set; }
    public PaginationModel? Pagination { get; private set; }
    
    public FollowingModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        FollowersList = new List<AuthorDTO>();
        FollowingList = new List<AuthorDTO>();
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page, string followType)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var myFollowing = await _authorRepository.GetFollowing(User.Identity?.Name!);
        var followingDtos = myFollowing.ToList();
        FollowingList = followingDtos;
        
        if(followType == "following")
        {
            var nFollowing = followingDtos.Count;
            Pagination = new PaginationModel(nFollowing, page);
            return Page();
        }
        
        var myFollowers = await _authorRepository.GetFollowers(User.Identity?.Name!);
        var followersDtos = myFollowers.ToList();
        FollowersList = followersDtos;
        
        var nAuthors = followersDtos.Count;
        Pagination = new PaginationModel(nAuthors, page);
        
        return Page();
    }
}
