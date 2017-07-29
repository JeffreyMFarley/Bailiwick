﻿using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class AcceptAndDone : IState
    {
        #region Effects

        public void OnEntry(IContext c)
        {
            c.AcceptCurrent();
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

        virtual public WordClassType GeneralWordClass
        {
            get { return WordClassType.Unclassified; }
        }

        virtual public IPhrase Build(IContext c)
        {
            return c.Previous.Build(c);
        }

        #endregion
    }
}
