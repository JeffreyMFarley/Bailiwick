using System.Collections.Generic;
using Esoteric.Collections;

namespace Bailiwick.Models
{
    public interface ISentenceNode : IIntervalProvider<int>
    {
        WordClassType GeneralWordClass { get; }
        WordInstance Head { get; }
    }
}
