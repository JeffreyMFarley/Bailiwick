using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Adjective : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Adjective; }
        }

        public override IPhrase Build(IContext c)
        {
            IPhrase result;
            if (c.Accepted.Any(x => x.IsSuperlative()) )
            {
                result = new SuperlativePhrase();  // TODO: Need to check for implied superlative "the most important"
            }
            else if (c.Accepted.Any(x => x.GeneralWordClass == WordClassType.GenitiveMarker))
            {
                result = new NounPhrase();  // TODO: Split and create two phrases
            }
            else if (c.TryConvert.ToAgent(c.Accepted.Last()))
            {
                result = new NounPhrase();
            }
            else if (c.Previous.GeneralWordClass == WordClassType.Article || c.Previous.GeneralWordClass == WordClassType.Determiner)
            {
                c.ForceConvert.ToAgent(c.Accepted.Last());
                result = new NounPhrase();
            }
            else
            {
                result = new AdjectivePhrase();
            }

            foreach (var w in c.Accepted)
                result.AddTail(w);

            return (result.Head != null) ? result : null;
        }

        #region Event Handlers

        public override IState OnAdjective(IContext c)
        {
            c.AcceptCurrent();
            return this;
        }

        public override IState OnAdverb(IContext c)
        {
            return AcceptAndDone;
        }

        public override IState OnConjunction(IContext c)
        {
            if( c.Current.IsCoordinatingConjunction() )
                return xSimpleList;

            return xConjunction;
        }

        public override IState OnDeterminer(IContext c)
        {
            if (c.Current.IsPostDeterminer())
                return Determiner;

            return base.OnDeterminer(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            if( c.Current.Normalized == "," )
                return xSimpleList;

            return base.OnPunctuation(c);
        }

        public override IState OnVerb(IContext c)
        {
            return ChooseVerbPsuedoState(c);
        }

        #endregion

    }
}
