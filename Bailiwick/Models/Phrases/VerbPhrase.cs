using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    public class VerbPhrase : PhraseBase
    {
        #region Phrase Specific Members

        public List<WordInstance> ComplementOf
        {
            get { return complementOf ?? (complementOf = new List<WordInstance>()); }
        }
        List<WordInstance> complementOf;

        public List<WordInstance> Modals
        {
            get { return modals ?? (modals = new List<WordInstance>()); }
        }
        List<WordInstance> modals;

        public List<WordInstance> Particles
        {
            get { return particles ?? (particles = new List<WordInstance>()); }
        }
        List<WordInstance> particles;

        public bool IsPassive
        {
            get
            {
                if (isPassive == null)
                {
                    isPassive = Modals.Any(x => x.Lemma == "be") && ClassColocates.Any(x => x.PartOfSpeech.Specific == "VVN");
                }

                return isPassive.Value;
            }
        }
        bool? isPassive;

        public bool IsFinite
        {
            get
            {
                if (isFinite == null)
                {
                    isFinite = !Modals.Any(x => x.GeneralWordClass == WordClassType.InfinitiveMarker);
                    isFinite &= Head != null && !Head.IsInfinite();
                }
                return isFinite.Value;
            }
        }
        bool? isFinite;

        #endregion
       
        override public IEnumerator<WordInstance> GetEnumerator()
        {
            return AdjunctColocates.Union(Particles)
                                   .Union(Modals)
                                   .Union(Negatives)
                                   .Union(ComplementOf)
                                   .OrderBy(x => x.StartIndex)
                                   .GetEnumerator();
        }

        override public WordClassType GeneralWordClass
        {
            get
            {
                return WordClassType.Verb;
            }
        }

        override public WordInstance Head
        {
            get
            {
                if (ClassColocates.Count > 0)
                    return ClassColocates.Last();

                return Modals.LastOrDefault(x => x.GeneralWordClass == WordClassType.Verb);
            }
        }

        override public bool IsSupported(WordInstance word)
        {
            return word.IsVerbPhrasePart();
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
                case WordClassType.Adposition:
                    ComplementOf.Insert(0, word);
                    break;

                case WordClassType.Adverb:
                    if( word.PartOfSpeech.Specific == "RP")
                        Particles.Insert(0, word);
                    else
                        Adjuncts.Insert(0, word);
                    AdjunctColocates.Insert(0, word);
                    break;

                case WordClassType.InfinitiveMarker:
                    Modals.Insert(0, word);
                    break;

                case WordClassType.Not:
                    Negatives.Insert(0, word);
                    break;

                case WordClassType.Verb:
                    if (word.IsAuxiallaryVerb())
                        Modals.Insert(0, word);
                    else
                    {
                        // Fix amibiguity between VVN and VVD
                        if( word.PartOfSpeech.Specific == "VVN" && !Modals.Any(x => x.Lemma == "have") )
                            word.PartOfSpeech = WordClasses.Specifics["VVD"];

                        ClassColocates.Insert(0, word);
                        AdjunctColocates.Insert(0, word);
                    }
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
                case WordClassType.Adposition:
                    ComplementOf.Add(word);
                    break;

                case WordClassType.Adverb:
                    if (word.PartOfSpeech.Specific == "RP")
                        Particles.Add(word);
                    else
                        Adjuncts.Add(word);
                    AdjunctColocates.Add(word);
                    break;

                case WordClassType.InfinitiveMarker:
                    Modals.Add(word);
                    break;

                case WordClassType.Not:
                    Negatives.Add(word);
                    break;

                case WordClassType.Verb:
                    if (word.IsAuxiallaryVerb())
                        Modals.Add(word);
                    else 
                    {
                        // Fix amibiguity between VVN and VVD
                        if (word.PartOfSpeech.Specific == "VVN" && !Modals.Any(x => x.Lemma == "have"))
                            word.PartOfSpeech = WordClasses.Specifics["VVD"];

                        ClassColocates.Add(word);
                        AdjunctColocates.Add(word);
                    }
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
                    if (word.PartOfSpeech.Specific == "RP")
                        Particles.Remove(word);
                    else
                        Adjuncts.Remove(word);
                    AdjunctColocates.Remove(word);
                    break;

                default:
                    return false;
            }
       
            return true;
        }

    }
}
