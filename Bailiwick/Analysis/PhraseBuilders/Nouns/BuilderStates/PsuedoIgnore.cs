using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class PsuedoIgnore : PsuedoBase
    {
        public override void OnEntry(IContext c)
        {
            //c.BufferCurrent();
        }

        public override PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Building; }
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Punctuation; }
        }

        public override IPhrase Build(IContext c)
        {
            return BaseState.InternalBuild(c);
        }

        #region IState Members

        override public IState OnAdjective(IContext c)
        {
            return c.Previous.OnAdjective(c);
        }

        override public IState OnAdverb(IContext c)
        {
            return c.Previous.OnAdverb(c);
        }

        public override IState OnAdposition(IContext c)
        {
            return c.Previous.OnAdposition(c);
        }

        public override IState OnArticle(IContext c)
        {
            return c.Previous.OnArticle(c);
        }

        public override IState OnConjunction(IContext c)
        {
            return c.Previous.OnConjunction(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            return c.Previous.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            return c.Previous.OnExistential(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            return c.Previous.OnGenitiveMarker(c);
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            return c.Previous.OnInfinitiveMarker(c);
        }

        public override IState OnInterjection(IContext c)
        {
            return c.Previous.OnInterjection(c);
        }

        public override IState OnLetter(IContext c)
        {
            return c.Previous.OnLetter(c);
        }

        public override IState OnNot(IContext c)
        {
            return c.Previous.OnNot(c);
        }

        override public IState OnNoun(IContext c)
        {
            return c.Previous.OnNoun(c);
        }

        public override IState OnNumber(IContext c)
        {
            return c.Previous.OnNumber(c);
        }

        public override IState OnPronoun(IContext c)
        {
            return c.Previous.OnPronoun(c);
        }

        override public IState OnPunctuation(IContext c)
        {
            return c.Previous.OnPunctuation(c);
        }

        public override IState OnUnclassified(IContext c)
        {
            return c.Previous.OnUnclassified(c);
        }

        public override IState OnVerb(IContext c)
        {
            return c.Previous.OnVerb(c);
        }

        #endregion
    }
}
