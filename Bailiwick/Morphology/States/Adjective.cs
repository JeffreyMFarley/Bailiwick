using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class Adjective : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            switch (w.GeneralWordClass)
            {
                case WordClassType.GenitiveMarker:
                case WordClassType.Verb:
                    return false;
            }

            var comparativeResult = AS.ComparativePhrase(c.Previous, c, w, percentage);
            if (comparativeResult != null)
                return comparativeResult == w.PartOfSpeech;

            // Use the predicate pipeline to knock out places where the adjective is not proper
            return PredicatePipeline.Execute(PredicatePipeline.VJN, c, w, percentage);
        }

        #endregion
    }
}
