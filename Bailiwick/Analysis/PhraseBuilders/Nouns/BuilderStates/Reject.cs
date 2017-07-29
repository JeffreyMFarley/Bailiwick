using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Reject : IState
    {
        #region Effects

        public void OnEntry(IContext c)
        {
            c.RejectAccepted();
        }

        public void OnExit(IContext c)
        {
        }

        #endregion

        #region IState Members

        public IState OnAdjective(IContext c)
        {
            return this;
        }

        public IState OnAdposition(IContext c)
        {
            return this;
        }

        public IState OnAdverb(IContext c)
        {
            return this;
        }

        public IState OnArticle(IContext c)
        {
            return this;
        }

        public IState OnConjunction(IContext c)
        {
            return this;
        }

        public IState OnDeterminer(IContext c)
        {
            return this;
        }

        public IState OnExistential(IContext c)
        {
            return this;
        }

        public IState OnGenitiveMarker(IContext c)
        {
            return this;
        }

        public IState OnInfinitiveMarker(IContext c)
        {
            return this;
        }

        public IState OnInterjection(IContext c)
        {
            return this;
        }

        public IState OnLetter(IContext c)
        {
            return this;
        }

        public IState OnNot(IContext c)
        {
            return this;
        }

        public IState OnNoun(IContext c)
        {
            return this;
        }

        public IState OnNumber(IContext c)
        {
            return this;
        }

        public IState OnPronoun(IContext c)
        {
            return this;
        }

        public IState OnPunctuation(IContext c)
        {
            return this;
        }

        public IState OnUnclassified(IContext c)
        {
            return this;
        }

        public IState OnVerb(IContext c)
        {
            return this;
        }

        public PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Done; }
        }

        public WordClassType GeneralWordClass
        {
            get { return WordClassType.Unclassified; }
        }

        public IPhrase Build(IContext c)
        {
            return null;
        }

        #endregion
    }
}
