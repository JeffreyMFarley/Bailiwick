using Bailiwick.Models;

namespace Bailiwick.Thoughts
{
    delegate IState TransitionPredicate(IContext c, WordInstance w);

    internal static class StateFactory
    {
        static internal IState Accumulate = new States.Accumulate();
        static internal IState Period = new States.Period();
        static internal IState EndOfSentence = new States.EOS();
        static internal IState Exclamation = new States.Exclamation();
        static internal IState Interrogative = new States.Interrogative();

        static internal IState HasTransition(TransitionPredicate[] predicates, IContext c, WordInstance w)
        {
            IState result = null;
            for (int i = 0; i < predicates.Length && result == null; i++)
            {
                result = predicates[i](c, w);
                if (result != null)
                    Engine.PredicateTally.Increment(predicates[i].Method.Name);
            }

            //TraceSources.States.CompressedTraceEvent(TraceEventType.Verbose, 4, "PipelineExecute: '{0}'", result);
            return result;
        }
    }
}
