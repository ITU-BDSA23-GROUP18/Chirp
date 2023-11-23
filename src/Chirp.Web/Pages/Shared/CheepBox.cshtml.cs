using System.Diagnostics.CodeAnalysis;

namespace Chirp.Web.Pages.Shared;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Razor Page")]
public class CheepBoxModel
{
    public CheepDTO Cheep { get; private set; }

    public CheepBoxModel(CheepDTO cheep)
    {
        Cheep = cheep;
    }
}
