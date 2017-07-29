using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models.Phrases
{
    public class SubordinatingPhrase : PhraseBase
    {
        public SubordinatingPhrase(IList<WordInstance> leadingWords, WordInstance subordinate)
        {
            foreach (var w in leadingWords)
                AddTail(w);

            AddTail(subordinate);
        }

        override public IEnumerator<WordInstance> GetEnumerator()
        {
            return AdjunctColocates.Union(Negatives)
                                   .OrderBy(x => x.StartIndex)
                                   .GetEnumerator();
        }

        override public WordClassType GeneralWordClass
        {
            get
            {
                return WordClassType.Conjunction;
            }
        }

        override public WordInstance Head
        {
            get
            {
                return ClassColocates.LastOrDefault();
            }
        }

        override public bool IsSupported(WordInstance word)
        {
            return word.GeneralWordClass == WordClassType.Adverb || word.IsSubordinatingConjunction();
        }

        override public void AddHead(WordInstance word)
        {
            if (!IsSupported(word))
                return;

            // Reset the indexes
            ResetIndexes();

            // Sort the word
            switch (word.GeneralWordClass)
            {
                case WordClassType.Adverb:
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Conjunction:
                    ClassColocates.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Not:
                    Negatives.Insert(0, word);
                    break;
            }
        }

        override public void AddTail(WordInstance word)
        {
            if (!IsSupported(word))
                return;

            // Set the indexes
            SetIndexes(word);

            // Sort the word
            switch (word.GeneralWordClass)
            {
                case WordClassType.Adverb:
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Conjunction:
                    ClassColocates.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Not:
                    Negatives.Add(word);
                    break;
            }
        }

        override public bool Remove(WordInstance word)
        {
            // Reset the indexes
            ResetIndexes();

            // Find the word
            switch (word.GeneralWordClass)
            {
                case WordClassType.Adverb:
                    Adjuncts.Remove(word);
                    AdjunctColocates.Remove(word);
                    break;

                case WordClassType.Not:
                    Negatives.Remove(word);
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}
