using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Determiner : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Determiner; }
        }

        #region Event Handlers

        public override IState OnAdposition(IContext c)
        {
            return base.OnAdposition(c);
        }

        public override IState OnArticle(IContext c)
        {
            return base.OnArticle(c);
        }

        public override IState OnConjunction(IContext c)
        {
            return xSimpleList;
        }

        public override IState OnDeterminer(IContext c)
        {
            // A string of after-determiners is OK
            if (c.Current.IsPostDeterminer())
            {
                c.AcceptCurrent();
                return this;
            }

            c.RejectAccepted();
            return base.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            return base.OnExistential(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            if( c.Accepted.Last().IsPostDeterminer() )
                return Genitive;

            return RejectAndDone;
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            return base.OnInfinitiveMarker(c);
        }

        public override IState OnNot(IContext c)
        {
            return base.OnNot(c);
        }

        public override IState OnPronoun(IContext c)
        {
            return base.OnPronoun(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            return base.OnPunctuation(c);
        }

        public override IState OnVerb(IContext c)
        {
            // Very likely this is really an adjective or noun
            return ChooseVerbPsuedoState(c);
        }

        #endregion

    }
}
