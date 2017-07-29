using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    public class NounPhrase : PhraseBase
    {
        #region Phrase Specific Members

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

        public List<WordInstance> Existentials
        {
            get { return existentials ?? (existentials = new List<WordInstance>()); }
        }
        List<WordInstance> existentials;

        public List<WordInstance> Numbers
        {
            get
            {
                if (numbers == null)
                {
                    numbers = new List<WordInstance>();
                }
                return numbers;
            }
        }
        List<WordInstance> numbers;

        public List<WordInstance> Pronouns
        {
            get { return pronouns ?? (pronouns = new List<WordInstance>()); }
        }
        List<WordInstance> pronouns;

        public bool IsPrepositionalPhrase 
        {
            get
            {
                if (isPrepositionalPhase == null)
                {
                    var first = this.FirstOrDefault();
                    isPrepositionalPhase = (first != null && first.GeneralWordClass == WordClassType.Adposition);
                }

                return isPrepositionalPhase.Value;
            }
        }
        bool? isPrepositionalPhase = null;

        #endregion

        protected override void ResetIndexes()
        {
            base.ResetIndexes();
            isPrepositionalPhase = null;
        }

        override public IEnumerator<WordInstance> GetEnumerator()
        {
            return ClassColocates.Union(ComplementOf)
                                 .Union(Adjuncts)
                                 .Union(Existentials)
                                 .Union(Numbers)
                                 .Union(Negatives)
                                 .Union(Pronouns)
                                 .OrderBy(x => x.StartIndex)
                                 .GetEnumerator();
        }

        override public WordClassType GeneralWordClass 
        {
            get
            {
                return WordClassType.Noun;
            }
        }

        override public WordInstance Head
        {
            get
            {
                if (ClassColocates.Count > 0)
                    return ClassColocates.Last();

                var head = 
                      ComplementOf
                      .Union(Numbers)
                      .Union(Existentials)
                      .Union(Pronouns)
                      .LastOrDefault(x => x.GeneralWordClass == WordClassType.Pronoun
                                       || x.GeneralWordClass == WordClassType.Existential
                                       || x.GeneralWordClass == WordClassType.Determiner
                                       || x.GeneralWordClass == WordClassType.Number);

                return head;
            }
        }

        override public bool IsSupported(WordInstance word)
        {
            return word.IsNounPhrasePart();
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
                    Adjuncts.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Adposition:
                    ComplementOf.Insert(0, word);
                    break;

                case WordClassType.Adverb:
                    Adjuncts.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Article:
                    ComplementOf.Insert(0, word);
                    break;

                case WordClassType.Determiner:
                    ComplementOf.Insert(0, word);
                    break;

                case WordClassType.Existential:
                    Existentials.Insert(0, word);
                    break;

                case WordClassType.GenitiveMarker:
                    ComplementOf.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Letter:
                    Adjuncts.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Not:
                    Negatives.Insert(0, word);
                    break;

                case WordClassType.Noun:
                    ClassColocates.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.Number:
                    Numbers.Insert(0, word);
                    break;

                case WordClassType.Pronoun:
                    Pronouns.Insert(0, word);
                    break;
            }
        }

        override public void AddTail(WordInstance word)
        {
            if ( !IsSupported(word) )
                return;

            // Set the indexes
            SetIndexes(word);

            // Sort the word
            switch (word.GeneralWordClass)
            {
                case WordClassType.Adjective:
                    Adjuncts.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Adposition:
                    ComplementOf.Add(word);
                    break;

                case WordClassType.Adverb:
                    Adjuncts.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Article:
                    ComplementOf.Add(word);
                    break;

                case WordClassType.Determiner:
                    ComplementOf.Add(word);
                    break;

                case WordClassType.Existential:
                    Existentials.Add(word);
                    break;

                case WordClassType.GenitiveMarker:
                    ComplementOf.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Letter:
                    Adjuncts.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Not:
                    Negatives.Add(word);
                    break;

                case WordClassType.Noun:
                    ClassColocates.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.Number:
                    Numbers.Add(word);
                    break;

                case WordClassType.Pronoun:
                    Pronouns.Add(word);
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
                case WordClassType.Adposition:
                    ComplementOf.Remove(word);
                    break;

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
