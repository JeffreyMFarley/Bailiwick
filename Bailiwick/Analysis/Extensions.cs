using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis
{
    static public class Extensions
    {
        #region WordInstance Extensions

        // A participle or simple past tense may be an adjective
        static public bool IsPotentialAdjective(this WordInstance w)
        {
            return w.IsParticiple() || w.PartOfSpeech.Specific == "VVD";
        }

        #endregion

        #region Verb Phrase Methods

        static public bool CanAcceptAsNextWord(this VerbPhrase p, WordInstance w)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Adposition:  // Never anything but lead of gerund
                    return false;

                // Can accept infinitive marker if empty or there is a 'not'
                case WordClassType.InfinitiveMarker:
                    return p.Count() == p.Negatives.Count();

                case WordClassType.Verb:
                    if ( w.IsGerund() )  // Try to determine participle vs gerund
                        return p.ClassColocates.Count == 0;
                    else
                        return true;
            }

            return true;
        }

        #endregion
    }
}
