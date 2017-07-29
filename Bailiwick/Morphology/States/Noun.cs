using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Noun : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            if (w.GeneralWordClass == WordClassType.Not)
                return false;

            if (w.Normalized != "whom" && w.IsObjectivePronoun())
                return false;

            //switch (w.PartOfSpeech.Specific)
            //{
                // This is true unless the main verb is divalent (I feed the _cows their_ food, I give the _cashier my_ money 
                //case "APPGE":
                //    n.Impossible = true;
                //    break;            
            //}

            // Use the predicate pipeline to knock out places where the noun is not proper
            return !pipelineNouns.Contains(c.Word.PartOfSpeech.Specific) ? true : PredicatePipeline.Execute(PredicatePipeline.VJN, c, w, percentage);
        }

        #endregion
        #region Predicate Properties

        static string[] pipelineNouns = { "NN1", "NN2" };

        internal static StatePredicate[] predicates = { 
                                                          ANounThat, TheNounAt
//                                                          , ArticleNounTO
//                                                          , AppgeNounEnd
                                                      };
        #endregion
        #region Predicates

        static internal WordClass ANounThat(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.Normalized == "a" && next.Normalized == "that")
                return WordClasses.Noun;

            return null;
        }

        // This method exposes words in the dictionary that are not also known as nouns -> finding, etc.
        static internal WordClass AppgeNounEnd(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.PartOfSpeech.Specific == "APPGE" && next.IsEndOfSentence() )
                return WordClasses.Noun;

            return null;
        }

        // This method exposes words in the dictionary that are not also known as nouns -> finding, etc.
        static internal WordClass ArticleNounTO(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.GeneralWordClass == WordClassType.Article && next.GeneralWordClass == WordClassType.InfinitiveMarker)
                return WordClasses.Noun;

            return null;
        }

        static internal WordClass TheNounAt(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.Normalized == "the" && next.Normalized == "at")
                return WordClasses.Noun;

            return null;
        }

        #endregion
    }
}
