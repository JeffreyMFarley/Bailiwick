using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Adverb : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch( w.GeneralWordClass )
            {
                case WordClassType.Determiner:
                case WordClassType.Existential:
                case WordClassType.Interjection:
                case WordClassType.Letter:
                case WordClassType.Noun:
                case WordClassType.Number:
                //case WordClassType.Pronoun:
                    return false;

                case WordClassType.GenitiveMarker:
                    return c.Word.PartOfSpeech.Specific == "RRQ";
            }

            var comparativeResult = AS.ComparativePhrase(c.Previous, c, w, percentage);
            if (comparativeResult != null )
                return comparativeResult == w.PartOfSpeech;

            return true;
        }

        #endregion
    }
}
