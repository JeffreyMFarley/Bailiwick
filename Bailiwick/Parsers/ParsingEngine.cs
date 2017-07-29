using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Parsers
{
    public class ParsingEngine
    {
        public ParsingEngine(ICorpus corpus)
        {
            Corpus = corpus;
        }

        public ICorpus Corpus { get; private set; }

        public IEnumerable<WordInstance> Parse(string morpheme)
        {
            var context = new Context() 
            {
                Corpus = Corpus
            };
            foreach(var wi in context.Parse(morpheme))
                yield return wi;
        }
    }
}
