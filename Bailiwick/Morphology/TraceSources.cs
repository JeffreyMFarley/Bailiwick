using System.Diagnostics;

namespace Bailiwick.Morphology
{
    /// <summary>
    /// Defines the traces sources for the namespace
    /// </summary>
    static public class TraceSources
    {
        static object _lock = new object();
        static bool SameAsLast(string test)
        {
            lock (_lock)
            {
                if (test != lastStateTrace)
                {
                    lastStateTrace = test;
                    return false;
                }
                else
                    return true;
            }
        }
        static string lastStateTrace = string.Empty;
	
        public static readonly TraceSource Classifier = new TraceSource("Bailiwick.Morphology.Classifier");

        public static readonly TraceSource States = new TraceSource("Bailiwick.Morphology.States");

        public static void CompressedTraceEvent(this TraceSource ts, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (ts.Switch.ShouldTrace(eventType))
            {
                var output = string.Format(format, args);
                if ( !SameAsLast(output) )
                    ts.TraceEvent(eventType, id, output);
            }
        }
    }
}
