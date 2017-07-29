using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Adposition : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch( w.GeneralWordClass )
            {
                case WordClassType.Adposition:  // There will be exceptions
                case WordClassType.Conjunction:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.InfinitiveMarker:
                case WordClassType.Interjection:
                case WordClassType.Not:
                    return false;

                case WordClassType.Verb:    // Gerunds are OK
                    return w.IsGerund();
            }

            return true;
        }

        #endregion
    }
}
