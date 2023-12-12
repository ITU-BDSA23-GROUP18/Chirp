using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly ICheepRepository _repository;
    public readonly IAuthorRepository _authorRepository;
    public List<CheepDTO> Cheeps { get; private set; }
    public List<AuthorDTO> Followers { get; private set; }
    public PaginationModel? Pagination { get; private set; }
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }   
    public string? ProfilePictureUrl { get; private set; }
    
    // for individual user/"Author" preferences:
    public bool IsDarkMode { get; private set; }
    public float FontSizeScale { get; private set; }
    
    public AboutMeModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        Cheeps = new List<CheepDTO>();
        Followers = new List<AuthorDTO>();
        _repository = repository;
        _authorRepository = authorRepository;
    }
    
    /// <summary>
    /// Gets the cheeps from the author with the given currentUserName
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;

        var authors = await _authorRepository.GetAuthorByName(User.Identity?.Name!);
        var author = authors.FirstOrDefault();
        if (author == null || User.Identity == null || !User.Identity.IsAuthenticated) {
            return RedirectToPage("public");
        }
        DisplayName = author.DisplayName;
        Email = author.Email != author.Name ? author.Email : "Email...";

        var followersList = (await _authorRepository.GetFollowers(author.Name)).ToList();
        var cheepsList = (await _repository.GetCheepFromAuthor(author.Name, page)).ToList();
        Cheeps.AddRange(cheepsList);
        Followers.AddRange(followersList);
        
        var nCheeps = Cheeps.Count;
        Pagination = new PaginationModel(nCheeps, page);

        ProfilePictureUrl = await _authorRepository.GetProfilePicture(author.Name);
        IsDarkMode = await _authorRepository.IsDarkMode(User.Identity?.Name!);
        FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity?.Name!);
        
        return Page();
    }
    
    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail
    /// </summary>
    /// <param name="newEmail"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostChangeEmail(string newEmail)
    {
        try
        {
            await _authorRepository.ChangeEmail(User.Identity?.Name!,newEmail );
            return RedirectToPage();
        }
        catch 
        {
            Console.WriteLine("Email: " + newEmail + " is already taken");
            return RedirectToPage();
        }
    }
    public async Task<IActionResult> OnPostChangeName(string newName){
        try
        {
            await _authorRepository.ChangeName(User.Identity?.Name!, newName);
            return RedirectToPage();
        }
        catch 
        {
            Console.WriteLine("Name: " + newName + " is already taken");
            return RedirectToPage();
        }
    }
    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostDeleteAccount(string authorName)
    {
        try
        {
            Console.WriteLine("the author name is:"+authorName);
            await _authorRepository.DeleteAuthor(authorName);

            //Need to signout the user
            return Redirect("MicrosoftIdentity/Account/SignOut");
        }
        catch 
        {
            Console.WriteLine("the author name is:"+authorName);
            Console.WriteLine("Author"+ authorName+"does not exist");
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
        catch 
        {
            return RedirectToPage();
        }
    }
    public async Task<IActionResult> OnPostSetDarkMode()
    {
        var isDarkMode = !await _authorRepository.IsDarkMode(User.Identity?.Name!);
        
        await _authorRepository.SetDarkMode(User.Identity?.Name!, isDarkMode);
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostSetFontSizeScale(float scale)
    {
        if (scale == 15)
        {
            scale = (float) 1.5;
        }
        if (scale < 1 || scale > 2)
        {
            return RedirectToPage();
        }
        await _authorRepository.SetFontSizeScale(User.Identity?.Name!, scale);
        return RedirectToPage();
    }
}
