using System.Collections.Generic;

namespace Bailiwick.Models
{
    public interface IPhrase : ISentenceNode, IEnumerable<WordInstance>
    {
        IList<WordInstance> ClassColocates { get; }
        IList<WordInstance> AdjunctColocates { get; }
        IList<WordInstance> Negatives { get; }

        bool IsSupported(WordInstance word);

        void AddHead(WordInstance word);
        void AddTail(WordInstance word);
        bool Remove(WordInstance word);
    }
}
