using System;
using Bailiwick.Models;

namespace Bailiwick.Thoughts.States
{
    internal class Interrogative : IState
    {
        TransitionPredicate[] predicates = { FinishedInterrogative };

        public bool Ready
        {
            get { return false; }
        }

        public void Enter(IContext c, WordInstance wi)
        {
            c.Next.Enqueue(wi);
            c.IsInterrogative = true;
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
            return 5;
        }

        #region Predicates

        static IState FinishedInterrogative(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech != WordClasses.Question) ? StateFactory.EndOfSentence : null;
        }

        #endregion
    }
}
