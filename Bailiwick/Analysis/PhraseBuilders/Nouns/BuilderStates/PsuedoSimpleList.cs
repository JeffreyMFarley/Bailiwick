using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class PsuedoSimpleList : PsuedoBase
    {
        public override void OnEntry(IContext c)
        {
            c.BufferCurrent();
        }

        public override PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Building; }
        }

        public override Models.WordClassType GeneralWordClass
        {
            get { return WordClassType.Unclassified; }
        }

        bool DoesPreviousMatch(IContext c, WordClassType wct)
        {
            if (c.Previous.GeneralWordClass == wct)
            {
                c.AcceptBuffer();
                return true;
            }

            return false;
        }

        public override IState OnAdjective(IContext c)
        {
            if( DoesPreviousMatch(c, WordClassType.Adjective) )
                return c.Previous;

            return base.OnAdjective(c);
        }

        public override IState OnAdposition(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Adposition))
                return c.Previous;
            
            return base.OnAdposition(c);
        }

        public override IState OnAdverb(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Adverb))
                return c.Previous;
            
            return base.OnAdverb(c);
        }

        public override IState OnArticle(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Article))
                return c.Previous;

            return base.OnArticle(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Determiner))
                return c.Previous;

            return base.OnDeterminer(c);
        }

        public override IState OnLetter(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Letter))
                return c.Previous;

            return base.OnLetter(c);
        }

        public override IState OnNoun(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Noun))
                return c.Previous;
            
            return base.OnNoun(c);
        }

        public override IState OnNumber(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Number))
                return c.Previous;

            return base.OnNumber(c);
        }

        public override IState OnPronoun(IContext c)
        {
            if (DoesPreviousMatch(c, WordClassType.Pronoun))
                return c.Previous;

            return base.OnPronoun(c);
        }
    }
}
