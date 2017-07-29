using Bailiwick.Models;
using System.Linq;

namespace Bailiwick.Analysis
{
    public class TryConvert : IConvert
    {
        public TryConvert(ICorpus corpus)
        {
            Corpus = corpus;
        }

        public ICorpus Corpus { get; private set; }

        public GlossComparer Comparer
        {
            get
            {
                return comparer ?? (comparer = new GlossComparer());
            }
        }
        GlossComparer comparer;

        #region IConvert members

        public bool ToAdposition(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Adposition
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if( result == null )
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToAdjective(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Adjective
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToAdverb(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Adverb
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToAgent(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) 
                       && (a.PartOfSpeech.General == WordClassType.Noun
                        || a.PartOfSpeech.General == WordClassType.Pronoun
                        || a.PartOfSpeech.General == WordClassType.Determiner
                        || a.PartOfSpeech.General == WordClassType.Existential
                        || a.IsSuperlative())
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToConjunction(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Conjunction
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToExistential(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Existential
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
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

            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Verb
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = WordClasses.Specifics["VVI"];
            return true;
        }

        public bool ToNoun(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Noun
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToNounPhrasePart(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General.IsNounPhrasePart()
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToParticle(WordInstance w)
        {
            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.Specific == "RP"
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        public bool ToVerb(WordInstance w)
        {
            if (w.Normalized == "'s" || w.Normalized == "\u2019s")
            {
                w.PartOfSpeech = WordClasses.Specifics["VBZ"];
                return true;
            }

            var q = from a in Corpus.OneGram[w.Normalized]
                    where !Comparer.Equals(a, w) && a.PartOfSpeech.General == WordClassType.Verb
                    orderby a.Count descending
                    select a.PartOfSpeech;

            var result = q.FirstOrDefault();
            if (result == null)
                return false;

            w.PartOfSpeech = result;
            return true;
        }

        #endregion
    }
}
