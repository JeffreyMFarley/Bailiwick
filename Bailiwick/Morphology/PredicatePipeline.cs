using System.Diagnostics;
using System.Linq;
using Bailiwick.Models;
using Bailiwick.Morphology.States;

namespace Bailiwick.Morphology
{
    delegate WordClass StatePredicate(IContext prev, IContext current, WordInstance next, double percentage);

    static internal class PredicatePipeline
    {
        static internal StatePredicate[] VJN
        {
            get
            {
                return vjn ?? ( vjn = Verb.predicates.Concat(Noun.predicates).ToArray());
            }
        }
        static StatePredicate[] vjn = null;

        static internal bool Execute(StatePredicate[] predicates, IContext current, WordInstance next, double percentage)
        {
            //if (percentage >= 0.99)
            //    return true;

            var prev = current.Previous;
            WordClass result = null;
            for (int i = 0; i < predicates.Length && result == null; i++)
            {
                result = predicates[i](prev, current, next, percentage);
                if (result != null )
                {
                    var key = string.Join("\t", 
                        predicates[i].Method.DeclaringType.Name, 
                        predicates[i].Method.Name, 
                        current.Word.GeneralWordClass);
                    Classifier.PredicateTally.Increment(key);
                }
            }

            TraceSources.States.CompressedTraceEvent(TraceEventType.Verbose, 4, "PipelineExecute: '{0}'", result);
            return result == null || result.General == current.Word.PartOfSpeech.General;
        }
    }
}
