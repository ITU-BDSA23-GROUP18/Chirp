using System.ComponentModel;

namespace Chirp.Core;

public enum ReactionType
{
    [Description("Thumb Up")]
    Good,
    
    [Description("Thumb Sideways")]
    Ish,
    
    [Description("Thumb Down")]
    Bad
}
