using System.ComponentModel;

namespace Chirp.core;

public enum ReactionType
{
    [Description("Thumb Up")]
    Good,
    
    [Description("Thumb Sideways")]
    Ish,
    
    [Description("Thumb Down")]
    Bad
}
