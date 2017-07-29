using System;
using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Parsers
{
    public interface IContext
    {
        ICorpus Corpus { get; }

        IParsingState State { get; }
        IParsingState Previous { get; }

        char Character { get; }
        char PreviousCharacter { get; }

        void PushHistory();
        void ResolvePsuedoState(IParsingState state, bool includeLastCharacterInHistory);
    }
}
