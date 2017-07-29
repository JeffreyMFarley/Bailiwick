using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    public class Null : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            TraceSources.States.CompressedTraceEvent(TraceEventType.Verbose, 0, "Null State {0} -> {1}", c.Word, w);
            return true;
        }

        #endregion
    }
}
