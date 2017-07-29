using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Initial : BaseState
    {
        override public  PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Standby;  }
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Start; }
        }

        override public void OnEntry(IContext c) {}

        override public void OnExit(IContext c) {}

        override public IState OnAdjective(IContext c)
        {
            return Adjective;
        }

        override public IState OnAdposition(IContext c)
        {
            return Adposition;
        }

        override public IState OnAdverb(IContext c)
        {
            return ChooseAdverbState(c);
        }

        override public IState OnArticle(IContext c)
        {
            return ChooseArticleState(c);
        }

        override public IState OnConjunction(IContext c)
        {
            c.RejectCurrent();
            return this;
        }

        override public IState OnDeterminer(IContext c)
        {
            return ChooseDeterminerState(c);
        }

        override public IState OnExistential(IContext c)
        {
            return Existential;
        }

        override public IState OnGenitiveMarker(IContext c)
        {
            c.RejectCurrent();
            return this;
        }

        override public IState OnInfinitiveMarker(IContext c)
        {
            return xInfinitive;
        }

        override public IState OnInterjection(IContext c)
        {
            return Interjection;
        }

        override public IState OnLetter(IContext c)
        {
            return Letter;
        }

        override public IState OnNot(IContext c)
        {
            return Not;
        }

        override public IState OnNoun(IContext c)
        {
            return Noun;
        }

        override public IState OnNumber(IContext c)
        {
            return Number;
        }

        override public IState OnPronoun(IContext c)
        {
            return Pronoun;
        }

        override public IState OnPunctuation(IContext c)
        {
            c.RejectCurrent();
            return this;
        }

        override public IState OnUnclassified(IContext c)
        {
            c.RejectCurrent();
            return this;
        }

        override public IState OnVerb(IContext c)
        {
            c.RejectCurrent();
            return this;
        }

        override public IPhrase Build(IContext c)
        {
            return null;
        }
    }
}
