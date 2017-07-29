using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    /// <summary>
    /// what, which, whatever, whichever
    /// </summary>
    internal class Determiner_Wh : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.GenitiveMarker:
                case WordClassType.Interjection:
                    return false;
            }

            return true;
        }

        #endregion
    }
}
