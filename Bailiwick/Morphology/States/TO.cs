using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class TO : IState
    {
        IState adposition = new Adposition();

        // http://www.learnenglish.de/grammar/verbsinfinitive.html
        string[] catenativeLead = {
                                      "afford", "agree", "appear", "arrange", "ask", "attempt", "care", "choose",
                                      "claim", "come", "consent", "dare", "decide", "demand", "deserve", "determine",
                                      "elect", "endeavour", "expect", "fail", "get", "guarantee", "hate", "help",
                                      "hesitate", "hope", "hurry", "incline", "intend", "learn", "long", "manage",
                                      "mean", "need", "offer", "plan", "prepare", "pretend", "promise", "refuse",
                                      "resolve", "say", "seem", "tend", "threaten", "want", "wish"
                                  };

        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            return (c.Word.GeneralWordClass == WordClassType.InfinitiveMarker) ? asInfinitiveMarker(w) : asAdposition(w, percentage, c);
        }

        #endregion
        #region Differentiators

        bool asInfinitiveMarker(WordInstance w)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.Adverb:
                    return true;  // but that isn't cool

                case WordClassType.Verb:
                    return !w.IsParticiple();
            }

            return false;
        }

        bool asAdposition(WordInstance w, double percentage, IContext c)
        {
            // Look for disqualifiers
            // TODO: More detailed tests since the initial results were equal failures with improvements
            //var prev = c.Previous.Word;
            //if (w.GeneralWordClass != WordClassType.Article && 
            //    prev.GeneralWordClass == WordClassType.Verb &&
            //    catenativeLead.Contains(prev.Lemma) )
            //    return false;

            return adposition.AcceptAsNext(w, percentage, c);
        }

        #endregion
    }
}
