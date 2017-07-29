using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class THAT : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            return PredicatePipeline.Execute(predicates, c, w, percentage);
        }

        #endregion
        #region Predicate Properties

        static StatePredicate[] predicates = { DeterminerPredictors, AsConjunction, DeterminerVerbs, Punctuation };
        static string[] DeterminerLeaders = { "VD0", "VDD", "RRQ", "DDQ" };
        static string[] ConjunctionFollowers = {
                                                   "APPGE", "AT", "AT1",
                                                   "CS",
                                                   "DB", "DD", "DD1", "DD2",
                                                   "EX",
                                                   "NN2", "NP1",
                                                   "PPHS1", "PPHS2"
                                               };
        static public WordClass DD1
        {
            get
            {
                return dd1 ?? (dd1 = WordClasses.Specifics["DD1"]);
            }
        }
        static WordClass dd1;

        static public WordClass CST
        {
            get
            {
                return cst ?? (cst = WordClasses.Specifics["CST"]);
            }
        }
        static WordClass cst;
        
        #endregion
        #region Predicates

        static WordClass DeterminerPredictors(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (DeterminerLeaders.Contains(prev.Word.PartOfSpeech.Specific))
                return DD1;

            return null;
        }
        
        static WordClass AsConjunction(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (ConjunctionFollowers.Contains(next.PartOfSpeech.Specific))
                return CST;

            return null;
        }

        static WordClass DeterminerVerbs(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (next.GeneralWordClass == WordClassType.Verb && 
                next.PartOfSpeech.Specific != "VVG" && 
                next.PartOfSpeech.Specific != "VVN")
                return DD1;

            return null;
        }

        static WordClass Punctuation(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if ( next.GeneralWordClass == WordClassType.Punctuation)
                return DD1;

            return null;
        }

        #endregion
    }
}
