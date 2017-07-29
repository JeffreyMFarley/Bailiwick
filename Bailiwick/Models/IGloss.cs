
namespace Bailiwick.Models
{
    /// <summary>
    /// Provides the base data contract for a word gloss
    /// </summary>
    public interface IGloss
    {
        string                  Lemma           { get; }
        string                  Normalized      { get; }
        WordClass    PartOfSpeech    { get; }
    }
}
