using Chirp.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; private set; }
    
    public PublicModel(ICheepRepository repository)
    {
        Cheeps = new List<CheepDTO>();
        _repository = repository;
    }

    public IActionResult OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        Cheeps = _repository.GetCheep(page).Result.ToList();
        return Page();
    }
    
    /*public async Task<IActionResult> OnPostCheep([FromQuery] int page, string message)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(Request.Scheme + "://" + Request.Host)
        };
        // make json data
        var data = new StringContent(
            JsonSerializer.Serialize(message),
            Encoding.UTF8,
            "application/json"
        );
        var response = client.PostAsync("/cheep", data).Result;
        response.EnsureSuccessStatusCode();
        return RedirectToPage("Public");
    }*/

    //on post cheep
    public IActionResult OnPostCheep([FromQuery] int page, string message)
    {
        _repository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }
}

