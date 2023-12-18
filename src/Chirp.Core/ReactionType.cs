namespace Chirp.Core;
using System.ComponentModel;

public enum ReactionType
{
    [Description("❤️")]
    Good,

    [Description("🕶️")]
    Ish,

    [Description("💩")]
    Bad,
}
