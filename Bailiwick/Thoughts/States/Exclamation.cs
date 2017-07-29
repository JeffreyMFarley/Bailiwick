using System;
using Bailiwick.Models;

namespace Bailiwick.Thoughts.States
{
    internal class Exclamation : IState
    {
        TransitionPredicate[] predicates = { FinishedExclamation };

        public bool Ready
        {
            get { return false; }
        }

        public void Enter(IContext c, WordInstance wi)
        {
            c.Next.Enqueue(wi);
        }

        public void OnWord(IContext c, WordInstance wi)
        {
            var symbol = c.Next.Dequeue();

            var newState = StateFactory.HasTransition(predicates, c, wi);
            if (newState == StateFactory.EndOfSentence)
            {
                c.Push(symbol);
                c.Next.Enqueue(wi);
            }
            else
            {
                var appended = new WordInstance(symbol.Instance + wi.Instance, symbol);
                c.Next.Enqueue(appended);
            }

            if (newState != null)
                c.Transfer(newState);
        }

        public override int GetHashCode()
        {
            return 4;
        }

        #region Predicates

        static IState FinishedExclamation(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech != WordClasses.Exclamation) ? StateFactory.EndOfSentence : null;
        }

        #endregion
    }
}
