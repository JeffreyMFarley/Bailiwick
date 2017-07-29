using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class PsuedoGerund : PsuedoBase
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Unclassified; }
        }

        public override void OnEntry(IContext c)
        {
            c.BufferCurrent();
        }

        public override IState OnAdjective(IContext c)
        {
            if (c.TryConvert.ToNounPhrasePart(c.Buffer.Last()) )
            {
                c.AcceptBuffer();
                return BaseState.Adjective;
            }

            return base.OnAdjective(c);
        }

        public override IState OnAdposition(IContext c)
        {
            if (c.TryConvert.ToAgent(c.Buffer.Last()))
            {
                c.ResolvePsuedoState(c.Previous, true);
                return BaseState.Initial.OnAdposition(c);
            }

            return base.OnAdposition(c);
        }

        public override IState OnAdverb(IContext c)
        {
            return base.OnAdverb(c);
        }

        public override IState OnArticle(IContext c)
        {
            return base.OnArticle(c);
        }

        public override IState OnConjunction(IContext c)
        {
            return base.OnConjunction(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            return base.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            return base.OnExistential(c);
        }

        public override IState OnNot(IContext c)
        {
            return base.OnNot(c);
        }

        public override IState OnNoun(IContext c)
        {
            if (c.TryConvert.ToNounPhrasePart(c.Buffer.Last()))
            {
                c.AcceptBuffer();
                return BaseState.Noun;
            }

            if (c.Previous.GeneralWordClass == WordClassType.Article || c.Previous.GeneralWordClass == WordClassType.Determiner)
            {
                c.ForceConvert.ToAdjective(c.Buffer.Last());
                c.AcceptBuffer();
                return BaseState.Noun;
            }

            return base.OnNoun(c);
        }

        public override IState OnNumber(IContext c)
        {
            if (c.TryConvert.ToNounPhrasePart(c.Buffer.Last()))
            {
                c.AcceptBuffer();
                return BaseState.Number;
            } 
            
            return base.OnNumber(c);
        }

        public override IState OnPronoun(IContext c)
        {
            if (c.TryConvert.ToNounPhrasePart(c.Buffer.Last()))
            {
                c.AcceptBuffer();
                return BaseState.Pronoun;
            } 
            
            return base.OnPronoun(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            if (c.Current.IsEndOfSentence())
            {
                if (c.TryConvert.ToAgent(c.Buffer.Last()))
                {
                    c.ResolvePsuedoState(c.Previous, true);
                    return BaseState.Initial.OnPunctuation(c);
                }
            }

            return base.OnPunctuation(c);
        }

        public override IState OnVerb(IContext c)
        {
            return base.OnVerb(c);
        }
    }
}
