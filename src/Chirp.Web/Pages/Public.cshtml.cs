﻿using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IReactionRepository _reactionRepository;
    private static List<CheepDTO> Cheeps { get; set; } = new();
    public PaginationModel? Pagination { get; private set; }
    public readonly IEnumerable<(string, string)> ReactionTypes;
    public string? ProfilePictureUrl { get; private set; }
    
    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _reactionRepository = reactionRepository;

        ReactionTypes = _reactionRepository.GetAllReactionTypes();
    }

    public async Task<IActionResult> OnGet([FromQuery] int page)
    {
        //If a page query is not given in the url set the page=1
        page = page <= 1 ? 1 : page;
        
        var nCheeps = _cheepRepository.CountCheeps().Result;
        Pagination = new PaginationModel(nCheeps, page);
        
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            ProfilePictureUrl = await _authorRepository.GetProfilePicture(User.Identity.Name!);
        }

        Cheeps = _cheepRepository.GetCheep(page).Result.ToList();
        
        return Page();
    }

    public List<CheepDTO> GetCheeps()
    {
        return Cheeps;
    }
    
    public IActionResult OnPostCheep(string message)
    {
        _cheepRepository.CreateCheep(message, User.Identity?.Name!);
        return RedirectToPage("Public");
    }

    public IActionResult OnPostChangeReaction(string cheepId, string reactionType)
    {
        var author = User.Identity?.Name!;
        if (!(User.Identity?.IsAuthenticated ?? false) || author == "") return Page();

        var cheepReactions = Cheeps.First(c => c.CheepId == cheepId).Reactions;

        if (cheepReactions.Any(r => r.Author == author))
        {
            Console.WriteLine($"REMOVING {reactionType} by {author}");
            _reactionRepository.RemoveReaction(cheepId, author);
            
            if (cheepReactions.First(r => r.Author == author).ReactionType == reactionType) return Page();
        }

        Console.WriteLine($"POSTING {reactionType} by {author}");
        _reactionRepository.CreateReaction(cheepId, author, reactionType);
        
        // author = User.Identity?.Name!;
        // if (author == "") return;
        // _reactionRepository.CreateReaction(cheepId, author, reactionType);
        return Page();
    }
    
    public async Task<string?> GetProfilePicture(string name)
    {
        return await _authorRepository.GetProfilePicture(name);
    }
}
