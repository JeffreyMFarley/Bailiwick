using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;
using System.Diagnostics;

namespace Bailiwick.Morphology.States
{
    internal class Verb : IState
    {
        WordClassType[] cannotFollowPerfect = new WordClassType[] {
                WordClassType.Adjective,
                WordClassType.Existential,
                WordClassType.GenitiveMarker,
                WordClassType.Letter,
                WordClassType.Noun,
                WordClassType.Pronoun
        };

        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                    return false;
            }

            // Two infinitives in a row is not correct
            if( c.Word.PartOfSpeech.Specific == "VV0" && w.PartOfSpeech.Specific == "VV0" )
                return false;

            // A finite followed by modal is not correct
            if( !c.Word.IsInfinite() && w.PartOfSpeech.Specific == "VM" )
                return false;

            // Get ready to process chains
            var prev = c.Previous.Word;

            // the perfect tense will not end on an agent (unless that's all there is?)
            if (prev.Lemma == "have" && c.Word.Lemma == "be" && cannotFollowPerfect.Contains(w.GeneralWordClass))
            {
                var decision = percentage > .90;
                if( !decision ) 
                    TraceSources.States.CompressedTraceEvent(TraceEventType.Verbose, 3, "Rejected {0} {1} {2}", prev, c.Word, w);
                return decision;
            }

            var comparativeResult = AS.ComparativePhrase(c.Previous, c, w, percentage);
            if (comparativeResult != null)
                return comparativeResult == w.PartOfSpeech;

            // Use the predicate pipeline to test the open verbs
            return (c.Word.IsClosedWordClass()) ? true : PredicatePipeline.Execute(PredicatePipeline.VJN, c, w, percentage);
        }

        #endregion
        #region Predicate Properties

        internal static StatePredicate[] predicates = { 
                                                          BeVVNAdposition, AdpositionVVGArticle, HaveVVNArticle, 
                                                          AdpositionVVGPronoun, PronounVVDPronoun
                                                      };

        static public WordClass VVD
        {
            get
            {
                return vvd ?? (vvd = WordClasses.Specifics["VVD"]);
            }
        }
        static WordClass vvd;

        static public WordClass VVG
        {
            get
            {
                return vvg ?? (vvg = WordClasses.Specifics["VVG"]);
            }
        }
        static WordClass vvg;

        static public WordClass VVN
        {
            get
            {
                return vvn ?? (vvn = WordClasses.Specifics["VVN"]);
            }
        }
        static WordClass vvn;

        #endregion
        #region Predicates

        static WordClass AdpositionVVGArticle(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.GeneralWordClass == WordClassType.Adposition && next.GeneralWordClass == WordClassType.Article)
                return VVG;

            return null;
        }

        static WordClass AdpositionVVGPronoun(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.GeneralWordClass == WordClassType.Adposition && next.GeneralWordClass == WordClassType.Pronoun)
                return VVG;

            return null;
        }

        static WordClass BeVVNAdposition(IContext prev, IContext current, WordInstance next, double percentage)
        {
            string[] afters = { "by", "out", "down" };

            if (prev.Word.Lemma == "be" && afters.Contains(next.Normalized))
                return VVN;

            return null;
        }

        static WordClass HaveVVNArticle(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.Lemma == "have" && next.GeneralWordClass == WordClassType.Article)
                return VVN;

            return null;
        }

        static WordClass PronounVVDPronoun(IContext prev, IContext current, WordInstance next, double percentage)
        {
            if (prev.Word.GeneralWordClass == WordClassType.Pronoun && next.GeneralWordClass == WordClassType.Pronoun)
                return VVD;

            return null;
        }

        #endregion
    }
}
