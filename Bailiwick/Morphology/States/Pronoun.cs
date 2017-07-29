using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Pronoun : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Determiner:
                case WordClassType.Existential:
                case WordClassType.Not:
                    return false;

                case WordClassType.GenitiveMarker:
                    return c.Word.PartOfSpeech.Specific == "PN1";

                case WordClassType.Verb:
                    return !w.IsParticiple();
            }

            return true;
        }

        #endregion
    }
}
