
namespace Bailiwick.Models
{
    /// <summary>
    /// Defines the operations available for converting one word class to another
    /// </summary>
    public interface IConvert
    {
        bool ToAdposition(WordInstance w);

        bool ToAdjective(WordInstance w);

        bool ToAdverb(WordInstance w);

        /// <summary>
        /// Convert the word to a class that can appear as the head of a noun phrase
        /// </summary>
        bool ToAgent(WordInstance w);

        bool ToConjunction(WordInstance w);

        bool ToExistential(WordInstance w);

        bool ToGenitive(WordInstance w);

        bool ToInfinitive(WordInstance w);

        bool ToNoun(WordInstance w);

        bool ToNounPhrasePart(WordInstance w);

        bool ToParticle(WordInstance w);

        bool ToVerb(WordInstance w);
    }
}
