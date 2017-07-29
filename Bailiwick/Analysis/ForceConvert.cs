using Bailiwick.Models;

namespace Bailiwick.Analysis
{
    public class ForceConvert : IConvert
    {
        public WordCaseStatus WordCase
        {
            get
            {
                return wordCase ?? (wordCase = new WordCaseStatus());
            }
        }
        WordCaseStatus wordCase;

        #region IConvert Members

        public bool ToAdposition(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.Adposition;
            return true;
        }

        public bool ToAdjective(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.Adjective;
            return true;
        }

        public bool ToAdverb(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.Adverb;
            return true;
        }

        public bool ToAgent(WordInstance w)
        {
            return ToNoun(w);
        }

        public bool ToConjunction(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.SubordinatingConjunction;
            return true;
        }

        public bool ToExistential(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.Existential;
            return true;
        }

        public bool ToGenitive(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.GenitiveMarker;
            return true;
        }

        public bool ToInfinitive(WordInstance w)
        {
            if (w.Lemma == "to")
            {
                w.PartOfSpeech = WordClasses.InfinitiveMarker;
                return true;
            }

            w.PartOfSpeech = WordClasses.Specifics["VVI"];
            return true;
        }

        public bool ToNoun(WordInstance w)
        {
            WordCase.Check(w.Instance);
            bool titled = WordCase.TitleCase ?? false;
            bool endsWithS = w.Normalized.EndsWith("s");

            if (titled && endsWithS)
                w.PartOfSpeech = WordClasses.Specifics["NP2"];
            else if( !titled && endsWithS )
                w.PartOfSpeech = WordClasses.Specifics["NN2"];
            else if (titled)
                w.PartOfSpeech = WordClasses.Specifics["NP1"];
            else
                w.PartOfSpeech = WordClasses.Noun;

            return true;
        }

        public bool ToNounPhrasePart(WordInstance w)
        {
            if (w.Normalized.EndsWith("ing"))
                w.PartOfSpeech = WordClasses.Adjective;
            else if (w.Normalized.EndsWith("ed"))
                w.PartOfSpeech = WordClasses.Adjective;
            else
                ToNoun(w);

            return true;
        }

        public bool ToParticle(WordInstance w)
        {
            w.PartOfSpeech = WordClasses.Specifics["RP"];
            return true;
        }

        public bool ToVerb(WordInstance w)
        {
            if (w.Normalized == "'s" || w.Normalized == "\u2019s")
                w.PartOfSpeech = WordClasses.Specifics["VBZ"];
            else if (w.Normalized.EndsWith("ing"))
                w.PartOfSpeech = WordClasses.Specifics["VVG"];
            else if (w.Normalized.EndsWith("ed"))
                w.PartOfSpeech = WordClasses.Specifics["VVD"];
            else if (w.Normalized.EndsWith("s"))
                w.PartOfSpeech = WordClasses.Specifics["VVZ"];
            else
                w.PartOfSpeech = WordClasses.Verb;

            return true;
        }

        #endregion
    }
}
