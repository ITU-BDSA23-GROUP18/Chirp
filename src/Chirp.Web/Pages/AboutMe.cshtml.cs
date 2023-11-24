using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> yourCheeps { get; private set; }
    public List<AuthorDTO> Followers { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    public string Email { get; private set; }
    
    public AboutMeModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        yourCheeps = new List<CheepDTO>();
        Followers = new List<AuthorDTO>();
        _repository = repository;
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var Author = await _authorRepository.GetAuthorByName(User.Identity?.Name!);
        Email = Author.FirstOrDefault().Email;

        
        foreach (var author in Author)
        {
            var Followers = _authorRepository.GetFollowers(author.Name).Result.ToList();
            var cheeps = _repository.GetCheepFromAuthor(author.Name, page).Result.ToList();
            yourCheeps.AddRange(cheeps);
            this.Followers.AddRange(Followers);
        }
        
        var nCheeps = yourCheeps.Count;
        Pagination = new PaginationModel(nCheeps, page);
        
        return Page();
    }
}
