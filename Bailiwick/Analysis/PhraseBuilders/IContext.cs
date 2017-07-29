using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders
{
    public interface IContext
    {
        // State Members
        IState State { get; }
        IState Previous { get; }

        void ResolvePsuedoState(IState state, bool includeBuffer);

        // WordInstance Members
        List<WordInstance> Accepted { get; }
        List<WordInstance> Buffer { get; }
        WordInstance Current { get; }

        void AcceptCurrent();   // Implies AcceptBuffer
        void BufferCurrent();
        void RejectCurrent();   // Implies RejectBuffer

        void PushAccepted();    // Implies RejectBuffer
        void RejectAccepted();  // Implies RejectCurrent and RejectBuffer

        void AcceptBuffer();

        // Convert Members
        IConvert TryConvert { get; }
        IConvert ForceConvert { get; }

    }
}
