using Bailiwick.Models;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Noun : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Noun; }
        }

        public override IState OnAdverb(IContext c)
        {
            if (c.Current.PartOfSpeech.Specific == "RA")
                return AcceptAndDone;

            // Temporal nouns can end with temporal adverbs
            var lw = c.Accepted.LastOrDefault();
            if (lw != null && lw.PartOfSpeech.Specific.StartsWith("NNT") && c.Current.PartOfSpeech.Specific.StartsWith("RT"))
                return AcceptAndDone;

            c.PushAccepted();
            return Initial.OnAdverb(c);
        }

        public override IState OnAdjective(IContext c)
        {
            // Comparative
            if (c.Current.IsComparative())
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
            c.AcceptCurrent();
            return this;
        }

        public override IState OnPronoun(IContext c)
        {
            c.PushAccepted();
            return Initial.OnPronoun(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            if (c.Current.IsQuote())
                return RejectAndDone;

            return base.OnPunctuation(c);
        }
    }
}
