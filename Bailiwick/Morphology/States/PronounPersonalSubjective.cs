using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class PronounPersonalSubjective : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Not:
                case WordClassType.Verb:
                    return true;

                case WordClassType.Adverb:
                    return true;   // May be able to restrict further

                case WordClassType.Punctuation:
                    return true;
            }

            return false;
        }

        #endregion
    }
}
