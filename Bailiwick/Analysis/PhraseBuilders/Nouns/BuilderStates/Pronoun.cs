using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Pronoun : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Pronoun; }
        }

        public override IState OnAdverb(IContext c)
        {
            if (c.Current.PartOfSpeech.Specific == "RA")
                return AcceptAndDone;

            // Degrees
            if (c.Current.IsComparative() || c.Current.IsSuperlative())
                return AcceptAndDone;

            c.PushAccepted();
            return Initial.OnAdverb(c);
        }

        public override IState OnAdjective(IContext c)
        {
            // Degrees
            if (c.Current.IsComparative() || c.Current.IsSuperlative())
                return AcceptAndDone;

            c.PushAccepted();
            return Initial.OnAdjective(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            return Genitive;
        }

        public override IState OnNoun(IContext c)
        {
            c.PushAccepted();
            return Initial.OnNoun(c);
        }

        public override IState OnPronoun(IContext c)
        {
            c.PushAccepted();
            return Initial.OnPronoun(c);
        }
    }
}
