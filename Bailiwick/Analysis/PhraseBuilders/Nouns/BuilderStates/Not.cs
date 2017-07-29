using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Not : BaseState
    {
        public override PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Starting; }
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Not; }
        }

        public override IPhrase Build(IContext c)
        {
            return null;
        }

        #region Event Handlers

        public override IState OnAdposition(IContext c)
        {
            return Adposition;
        }

        public override IState OnArticle(IContext c)
        {
            return ChooseArticleState(c);
        }

        public override IState OnConjunction(IContext c)
        {
            return Reject;
        }

        public override IState OnDeterminer(IContext c)
        {
            return Initial.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            return Reject;
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            return Reject;
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            return Reject;
        }

        public override IState OnNot(IContext c)
        {
            return Reject;
        }

        public override IState OnPunctuation(IContext c)
        {
            return ChoosePunctuationState(c, Reject);
        }

        public override IState OnUnclassified(IContext c)
        {
            return Reject;
        }

        public override IState OnVerb(IContext c)
        {
            return Reject;
        }

        #endregion

    }
}
