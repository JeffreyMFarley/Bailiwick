using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Existential : AcceptAndDone
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Existential; }
        }

        public override IPhrase Build(IContext c)
        {
            return BaseState.InternalBuild(c);
        }
    }
}
