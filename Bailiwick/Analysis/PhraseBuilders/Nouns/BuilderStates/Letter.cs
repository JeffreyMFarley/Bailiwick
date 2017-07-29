using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Letter : Noun
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Noun; }
        }
    }
}
