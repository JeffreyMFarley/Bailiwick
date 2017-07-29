using Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns
{
    public class Interjection : IState
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

        public PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Building; }
        }

        public WordClassType GeneralWordClass
        {
            get { return WordClassType.Interjection; }
        }

        public IPhrase Build(IContext c)
        {
            return new InterjectionPhrase(c.Accepted);
        }

        #region Event Handlers

        public IState OnAdjective(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnAdjective(c);
        }

        public IState OnAdposition(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnAdposition(c);
        }

        public IState OnAdverb(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnAdverb(c);
        }

        public IState OnArticle(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnArticle(c);
        }

        public IState OnConjunction(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnConjunction(c);
        }

        public IState OnDeterminer(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnDeterminer(c);
        }

        public IState OnExistential(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnExistential(c);
        }

        public IState OnGenitiveMarker(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnGenitiveMarker(c);
        }

        public IState OnInfinitiveMarker(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnInfinitiveMarker(c);
        }

        public IState OnInterjection(IContext c)
        {
            c.AcceptCurrent();
            return this;
        }

        public IState OnLetter(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnLetter(c);
        }

        public IState OnNot(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnNot(c);
        }

        public IState OnNoun(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnNoun(c);
        }

        public IState OnNumber(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnNumber(c);
        }

        public IState OnPronoun(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnPronoun(c);
        }

        public IState OnPunctuation(IContext c)
        {
            if (BaseState.IgnorablePunctuation(c))
            {
                c.AcceptCurrent();
                return this;
            }

            return BaseState.RejectAndDone;
        }

        public IState OnUnclassified(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnUnclassified(c);
        }

        public IState OnVerb(IContext c)
        {
            c.PushAccepted();
            return BaseState.Initial.OnVerb(c);
        }

        #endregion
    }
}
