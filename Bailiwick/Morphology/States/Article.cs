using System.Diagnostics;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Article : IState
    {
        static internal bool TooManyNouns(IContext prev, IContext current, WordInstance next, double percentage)
        {
            var pw = prev.Word;

            // One of these is a noun and one is a verb or adjective (and not "Bob the Builder")
            if (next.GeneralWordClass == WordClassType.Noun &&
                pw.GeneralWordClass == WordClassType.Noun &&
                !pw.PartOfSpeech.Specific.StartsWith("NP") && 
                prev.Percentage < 1.0 && percentage < 1.0
                )
            {
                TraceSources.States.CompressedTraceEvent(TraceEventType.Warning, 1, "Rejected {0} {1} {2}", pw, current.Word, next);
                return true;
            }

            return false;
        }

        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Adposition:
                case WordClassType.Article:
                case WordClassType.Conjunction:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.InfinitiveMarker:
                case WordClassType.Not:
                case WordClassType.Punctuation:
                case WordClassType.Unclassified:
                    return false;

                case WordClassType.Verb:
                    return w.IsParticiple();
            }

            if (w.GeneralWordClass == WordClassType.Determiner)
                return w.IsPostDeterminer();

            // This form of NOUN ARTICLE NOUN is OK
            if (c.Previous.Word.IsTemporal() && c.Word.PartOfSpeech.Specific == "AT1" && w.IsTemporal())
                return true;

            if( TooManyNouns(c.Previous, c, w, percentage) )
                return false;

            return true;
        }

        #endregion
    }
}
