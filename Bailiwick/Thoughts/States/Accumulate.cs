using System;
using Bailiwick.Models;

namespace Bailiwick.Thoughts.States
{
    internal class Accumulate : IState
    {
        TransitionPredicate[] predicates = { ToPeriod, ToInterrogative, ToExclamation, ToEndOfSentence };

        public bool Ready
        {
            get { return false; }
        }

        public void Enter(IContext c, WordInstance wi)
        {
        }

        public void OnWord(IContext c, WordInstance wi)
        {
            var newState = StateFactory.HasTransition(predicates, c, wi);

            if (newState == null || newState == StateFactory.EndOfSentence)
                c.Push(wi);

            if (newState != null)
                c.Transfer(newState);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        #region Predicates

        static IState ToPeriod(IContext c, WordInstance w)
        {
            return (w.Instance == ".") ? StateFactory.Period : null;
        }

        static IState ToExclamation(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.Exclamation) ? StateFactory.Exclamation : null;
        }

        static IState ToInterrogative(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.Question) ? StateFactory.Interrogative : null;
        }

        static IState ToEndOfSentence(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.NewLine) ? StateFactory.EndOfSentence : null;
        }

        #endregion
    }
}
