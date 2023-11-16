namespace Chirp.Core;

using System.ComponentModel;

public enum ReactionType
{
    [Description("Thumb Up")]
    Good,
    
    [Description("Thumb Sideways")]
    Ish,
    
    [Description("Thumb Down")]
    Bad
}
