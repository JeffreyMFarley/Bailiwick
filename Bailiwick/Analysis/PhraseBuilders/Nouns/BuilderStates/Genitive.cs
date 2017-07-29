using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Genitive : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.GenitiveMarker; }
        }
    }
}
