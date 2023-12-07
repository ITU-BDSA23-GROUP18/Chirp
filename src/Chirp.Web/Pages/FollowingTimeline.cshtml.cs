using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class FollowingTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> Cheeps { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public bool IsDarkMode { get; private set; }
    
    public float FontSizeScale { get; private set; }
    
    public FollowingTimelineModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
        _authorRepository = authorRepository;
        IsDarkMode = false;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
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
}
