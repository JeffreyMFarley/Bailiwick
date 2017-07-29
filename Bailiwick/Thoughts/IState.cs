using Bailiwick.Models;
using System;

namespace Bailiwick.Thoughts
{
    internal interface IState
    {
        // Signals
        bool Ready { get; }

        // Actions
        void Enter(IContext c, WordInstance wi);

        // Events
        void OnWord(IContext c, WordInstance wi);
    }
}
