using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    public class AdjectivePhrase : PhraseBase
    {
        public AdjectivePhrase() { }

        public AdjectivePhrase(IEnumerable<WordInstance> words)
        {
            foreach (var w in words)
                AddTail(w);
        }

        #region Phrase Specific-Members

        public List<WordInstance> ComplementOf
        {
            get
            {
                if (complementOf == null)
                {
                    complementOf = new List<WordInstance>();
                }
                return complementOf;
            }
        }
        List<WordInstance> complementOf;

        #endregion

        #region IPhrase Members

        override public IEnumerator<WordInstance> GetEnumerator()
        {
            return AdjunctColocates.Union(Negatives)
                                   .Union(ComplementOf)
                                   .OrderBy(x => x.StartIndex)
                                   .GetEnumerator();
        }

        override public WordClassType GeneralWordClass
        {
            get
            {
                return WordClassType.Adjective;
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
            return word.IsAdjectivePhrasePart();
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
                    ClassColocates.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Adverb:
                case WordClassType.Number:
                    Adjuncts.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Adposition:
                case WordClassType.Article:
                case WordClassType.Determiner:
                    ComplementOf.Insert(0, word);
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
                case WordClassType.Adjective:
                    ClassColocates.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Adverb:
                case WordClassType.Number:
                    Adjuncts.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Adposition:
                case WordClassType.Article:
                case WordClassType.Determiner:
                    ComplementOf.Add(word);
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
                case WordClassType.Number:
                    Adjuncts.Remove(word);
                    AdjunctColocates.Remove(word);
                    break;

                case WordClassType.Adposition:
                case WordClassType.Article:
                case WordClassType.Determiner:
                    ComplementOf.Remove(word);
                    break;

                case WordClassType.Not:
                    Negatives.Remove(word);
                    break;

                default:
                    return false;
            }

            return true;
        }

        #endregion 
    }
}
