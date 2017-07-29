using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Thoughts
{
    internal interface IContext
    {
        // Buffer Members
        IReadOnlyList<WordInstance> Buffer { get; }
        void Push(WordInstance w);
        WordInstance Pop();
        WordInstance Peek { get; }

        // Properties
        Queue<WordInstance> Next { get; }
        IState Status { get; }
        bool IsInterrogative { get; set; }

        // Methods
        void Transfer(IState newState);
    }
}
