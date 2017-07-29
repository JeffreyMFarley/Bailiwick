using System.Linq;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Adposition : BaseState
    {
        public override PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Starting; }
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Adposition; }
        }

        public override IPhrase Build(IContext c)
        {
            if( IsEndingPunctuation(c) )
              return new EllipsisNounPhrase(c.Accepted, null);

            return null;
        }

        #region Event Handlers

        public override IState OnAdposition(IContext c)
        {
            c.AcceptCurrent();
            return this;
        }

        public override IState OnArticle(IContext c)
        {
            return ChooseArticleState(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            return ChooseDeterminerState(c);
        }

        public override IState OnVerb(IContext c)
        {
            return ChooseVerbPsuedoState(c);
        }

        #endregion

    }
}
