using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Chirp.Web.Pages;
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Razor Page")]

public class AboutMeModel : PageModel
{
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    // for individual user/"Author" preferences:
    public bool IsDarkMode { get; private set; }
    public float FontSizeScale { get; private set; }
    private readonly IAuthorRepository _authorRepository;

    public AboutMeModel(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Gets the cheeps from the author with the given currentUserName.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnGet()
    {
        var authors = await _authorRepository.GetAuthorByName(User.Identity?.Name!);
        var author = authors.FirstOrDefault();
        if (author == null || User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToPage("public");
        }

        DisplayName = author.DisplayName;
        Email = author.Email != author.Name ? author.Email : "Email...";

        ProfilePictureUrl = await _authorRepository.GetProfilePicture(author.Name);
        IsDarkMode = await _authorRepository.IsDarkMode(User.Identity?.Name!);

        FontSizeScale = await _authorRepository.GetFontSizeScale(User.Identity?.Name!);

        return Page();
    }

    /// <summary>
    /// Changes the email of the author with the given currentUserName to the given newEmail.
    /// </summary>
    /// <param name="newEmail"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostChangeEmail(string newEmail)
    {
        try
        {
            await _authorRepository.ChangeEmail(User.Identity?.Name!, newEmail);
            return RedirectToPage();
        }
        catch (ArgumentException e)
        {
            return RedirectToPage("aboutme", new { error = e.Message ?? "Email Already in use." });
        }
        catch
        {
            return RedirectToPage("aboutme", new { error = "Error Chaning Email" });
        }
    }

    public async Task<IActionResult> OnPostChangeName(string newName)
    {
        try
        {
            await _authorRepository.ChangeName(User.Identity?.Name!, newName);
            return RedirectToPage();
        }
        catch (ArgumentException e)
        {
            return RedirectToPage("aboutme", new { error = e.Message ?? "Username Already in use." });
        }
        catch
        {
            return RedirectToPage("aboutme", new { error = "Error Chaning Username" });
        }
    }

    /// <summary>
    /// Follows the author with the given followName from the author with the given currentUserName.
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnPostDeleteAccount(string authorName)
    {
        try
        {
            await _authorRepository.DeleteAuthor(authorName);

            // Need to signout the user
            return Redirect("MicrosoftIdentity/Account/SignOut");
        }
        catch
        {
            return RedirectToPage("aboutme", new { error = "Could not delete user. Author " + authorName + " does not exist" });
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
            scale = 1.5F;
        }

        if (scale < 1 || scale > 2)
        {
            return RedirectToPage();
        }

        await _authorRepository.SetFontSizeScale(User.Identity?.Name!, scale);
        return RedirectToPage();
    }
}
