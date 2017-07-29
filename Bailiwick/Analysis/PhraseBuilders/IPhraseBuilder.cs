using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders
{
    public interface IPhraseBuilder
    {
        PhraseBuilderStatus State { get; }
        List<IPhrase> Phrases { get; }

        void Reset();
        PhraseBuilderStatus Process(WordInstance word);
        void Finished();
    }
}
