using System.Linq;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Adverb : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Adverb; }
        }

        public override IPhrase Build(IContext c)
        {
            var result = new AdverbPhrase(c.Accepted);
            return (result.Head != null) ? result : null;
        }

        #region Event Handlers

        public override IState OnAdverb(IContext c)
        {
            var state = ChooseAdverbState(c);

            if( state == this )
              c.AcceptCurrent();

            return state;
        }

        public override IState OnConjunction(IContext c)
        {
            if (c.Current.IsCoordinatingConjunction())
                return xSimpleList;

            return xConjunction;
        }

        public override IState OnDeterminer(IContext c)
        {
            if (c.Current.IsPreDeterminer())
                return BeforeDeterminer;

            return base.OnDeterminer(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            // today's, tomorrow's
            if (c.Accepted.Last().PartOfSpeech.Specific.StartsWith("RT") )
                return Genitive;

            return base.OnGenitiveMarker(c);
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            // The adverb belongs to a verb
            return Reject;
        }

        public override IState OnVerb(IContext c)
        {
            var state = ChooseVerbPsuedoState(c);

            // This adverb belongs to a verb
            if (state == RejectAndDone)
                state = Reject;

            return state;
        }

        #endregion
    }
}
