using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models.Phrases
{
    public class AdverbPhrase : PhraseBase
    {
        public AdverbPhrase(IList<WordInstance> words)
        {
            foreach (var w in words)
                AddTail(w);
        }

        override public IEnumerator<WordInstance> GetEnumerator()
        {
            return AdjunctColocates.Union(Negatives)
                                   .Union(TheRest)
                                   .OrderBy(x => x.StartIndex)
                                   .GetEnumerator();
        }

        override public WordClassType GeneralWordClass
        {
            get
            {
                return WordClassType.Adverb;
            }
        }

        override public WordInstance Head
        {
            get
            {
                return ClassColocates.LastOrDefault();
            }
        }

        public List<WordInstance> TheRest
        {
            get { return theRest ?? (theRest = new List<WordInstance>()); }
        }
        List<WordInstance> theRest;

        override public bool IsSupported(WordInstance word)
        {
            return true;
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
                case WordClassType.Adjective:
                case WordClassType.Number:
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Adverb:
                    ClassColocates.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Not:
                    Negatives.Insert(0, word);
                    break;

                default:
                    TheRest.Insert(0, word);
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
                case WordClassType.Adjective:
                case WordClassType.Number:
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Adverb:
                    ClassColocates.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Not:
                    Negatives.Add(word);
                    break;

                default:
                    TheRest.Add(word);
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
                case WordClassType.Adjective:
                case WordClassType.Number:
                    Adjuncts.Remove(word);
                    AdjunctColocates.Remove(word);
                    break;

                case WordClassType.Adverb:
                    return false;

                case WordClassType.Not:
                    Negatives.Remove(word);
                    break;

                default:
                    TheRest.Remove(word);
                    break;
                    
            }

            return true;
        }
    }
}
