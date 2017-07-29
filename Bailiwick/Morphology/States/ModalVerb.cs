using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class ModalVerb : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Determiner:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.Interjection:
                case WordClassType.Letter:
                case WordClassType.Noun:
                case WordClassType.Number:
                    return false;
            }

            switch (w.PartOfSpeech.Specific)
            {
                case "PPHO1":
                case "PPHS1":
                case "PPHS2":
                case "PN1":
                case "PPX1":
                case "PPX2":
                    return true;
            }

            if (w.GeneralWordClass == WordClassType.Pronoun)
                return false;

            return true;
        }

        #endregion
    }
}
