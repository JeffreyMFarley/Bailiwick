using System.Diagnostics;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Determiner : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch( w.GeneralWordClass )
            {
                case WordClassType.GenitiveMarker:
                    return c.Word.PartOfSpeech.Specific == "DA";

                case WordClassType.InfinitiveMarker:
                case WordClassType.Interjection:
                //case WordClassType.Punctuation:  // In this case, it is probably more of a prounoun
                    return false;

                case WordClassType.Verb:
                    return w.IsParticiple();

            }

            var comparativeResult = AS.ComparativePhrase(c.Previous, c, w, percentage);
            if ( comparativeResult != null )
                return comparativeResult == w.PartOfSpeech;

            if ( Article.TooManyNouns(c.Previous, c, w, percentage) )
                return false;

            return true;
        }

        #endregion
    }
}
