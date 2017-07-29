using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    public class InterjectionPhrase : PhraseBase
    {
        public InterjectionPhrase(IEnumerable<WordInstance> words)
        {
            foreach(var w in words)
                AddTail(w);
        }

        public override IEnumerator<WordInstance> GetEnumerator()
        {
            return AdjunctColocates.OrderBy(x => x.StartIndex)
                                   .GetEnumerator();
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Interjection; }
        }

        public override WordInstance Head
        {
            get 
            { 
                return ClassColocates.LastOrDefault(); 
            }
        }

        public override bool IsSupported(WordInstance word)
        {
            return true;
        }

        public override void AddHead(WordInstance word)
        {
            if (word.GeneralWordClass == WordClassType.Interjection)
                ClassColocates.Insert(0, word);
            else 
                Adjuncts.Insert(0, word);

            AdjunctColocates.Insert(0, word);
        }

        public override void AddTail(WordInstance word)
        {
            if (word.GeneralWordClass == WordClassType.Interjection)
                ClassColocates.Add(word);
            else
                Adjuncts.Add(word);

            AdjunctColocates.Add(word);
        }

        public override bool Remove(WordInstance word)
        {
            return false;
        }
    }
}
