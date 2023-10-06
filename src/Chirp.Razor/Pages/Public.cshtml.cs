using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Repositories.DTO;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IRepository<Cheep, MainCheepDTO, Author> _repository;
    
    public List<MainCheepDTO> Cheeps {get; private set;}

    public PublicModel(IRepository<Cheep,MainCheepDTO, Author> repository)
    {
        _repository = repository;
    }
    
    public IActionResult OnGet([FromQuery]int page)
    {
        //If a page query is not given in the url set the page=1
        Cheeps =  _repository.Get(page).Result.ToList();
        return Page();
    }
}
