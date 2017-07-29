using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Number : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Number; }
        }

        public override IState OnAdjective(IContext c)
        {
            return Adjective;
        }

        public override IState OnAdposition(IContext c)
        {
            return Adposition;
        }

        public override IState OnAdverb(IContext c)
        {
            // Degrees
            if (c.Current.IsComparative() || c.Current.IsSuperlative())
                return AcceptAndDone;

            return RejectAndDone;
        }

        public override IState OnNumber(IContext c)
        {
            c.AcceptCurrent();
            return this;
        }
    }
}
