using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class AS : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            return PredicatePipeline.Execute(predicates, c, w, percentage);
        }

        #endregion
        #region Predicate Properties

        static StatePredicate[] predicates = { AsConjunction, ConjunctionVerbs, Determiners };
        static string[] comparatives = { "DA", "JJ", "RR", "VVG", "VVN" };

        static public WordClass CSA
        {
            get
            {
                return csa ?? (csa = WordClasses.Specifics["CSA"]);
            }
        }
        static WordClass csa;	

        #endregion
        #region Predicates

        static WordClass AsConjunction(IContext prev, IContext current, WordInstance next, double percentage)
        {
            switch (next.GeneralWordClass)
            {
                case WordClassType.Article:
                case WordClassType.Conjunction:
                case WordClassType.Existential:
                case WordClassType.InfinitiveMarker:
                case WordClassType.Noun:
                case WordClassType.Number:
                case WordClassType.Pronoun:
                    return CSA;
            }

            return null;
        }

        static WordClass ConjunctionVerbs(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (next.GeneralWordClass == WordClassType.Verb &&
                next.PartOfSpeech.Specific != "VVG" &&
                next.PartOfSpeech.Specific != "VVN")
                return CSA;

            return null;
        }

        static WordClass Determiners(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (next.GeneralWordClass == WordClassType.Determiner &&
                next.PartOfSpeech.Specific != "DA" )
                return CSA;

            return null;
        }

        static internal WordClass ComparativePhrase(IContext prev, IContext current, WordInstance next, double percentage)
        {
            var pw = prev.Word;

            if( pw.Lemma == "as" && pw.GeneralWordClass == WordClassType.Adverb && next.Lemma == "as"
                && comparatives.Contains(current.Word.PartOfSpeech.Specific))
                return CSA;

            return null;
        }

        #endregion
    }
}
