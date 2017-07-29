using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Not : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch( w.GeneralWordClass )
            {
                case WordClassType.Adposition:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.Interjection:
                    return false;

                case WordClassType.Conjunction:
                    return w.IsSubordinatingConjunction();
            }

            return true;
        }

        #endregion
    }
}
