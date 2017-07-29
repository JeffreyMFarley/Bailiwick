using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class PsuedoInfinitive : PsuedoBase
    {
        public override void OnEntry(IContext c)
        {
            c.BufferCurrent();
        }

        public override PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Starting; }
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.InfinitiveMarker; }
        }

        bool ConvertBuffer(IContext c)
        {
            return c.Buffer.All(x => x.GeneralWordClass == WordClassType.Adverb || c.TryConvert.ToAdposition(x) );
        }

        public override IState OnAdjective(IContext c)
        {
            if ( ConvertBuffer(c) )
            {
                c.AcceptBuffer();
                return BaseState.Adjective;
            }

            return base.OnAdjective(c);
        }

        public override IState OnAdverb(IContext c)
        {
            c.BufferCurrent();
            return this;
        }

        public override IState OnArticle(IContext c)
        {
            if (ConvertBuffer(c))
            {
                c.AcceptBuffer();
                return BaseState.ChooseArticleState(c);
            }

            return base.OnArticle(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            if (ConvertBuffer(c))
            {
                c.AcceptBuffer();
                return BaseState.Initial.OnDeterminer(c);
            }

            return base.OnDeterminer(c);
        }

        public override IState OnNoun(IContext c)
        {
            if (ConvertBuffer(c))
            {
                c.AcceptBuffer();
                return BaseState.Noun;
            }

            return base.OnNoun(c);
        }

        public override IState OnNumber(IContext c)
        {
            if (ConvertBuffer(c))
            {
                c.AcceptBuffer();
                return BaseState.Number;
            }

            return base.OnNumber(c);
        }

        public override IState OnPronoun(IContext c)
        {
            if (ConvertBuffer(c))
            {
                c.AcceptBuffer();
                return BaseState.Pronoun;
            }

            return base.OnPronoun(c);
        }

        public override IState OnVerb(IContext c)
        {
            return base.OnVerb(c);
        }
    }
}
