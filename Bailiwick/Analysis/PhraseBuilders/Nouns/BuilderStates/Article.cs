using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class Article : Initial
    {
        public override void OnEntry(IContext c)
        {
            c.AcceptCurrent();
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Article; }
        }

        public override IPhrase Build(IContext c)
        {
            return BaseState.InternalBuild(c);
        }

        bool CanConvertPossessiveArticle(IContext c)
        {
            var last = c.Accepted.Last();

            if (last.PartOfSpeech.Specific == "APPGE") 
            { 
                if( last.Normalized == "her" )
                    last.PartOfSpeech = WordClasses.Specifics["PPHO1"];
                else
                    last.PartOfSpeech = WordClasses.Specifics["PPGE"];
                return true;
            }

            return false;
        }

        #region Event Handlers

        public override IState OnAdposition(IContext c)
        {
            if( !CanConvertPossessiveArticle(c) )
                c.RejectAccepted();
            else
                c.PushAccepted();

            return base.OnAdposition(c);
        }

        public override IState OnArticle(IContext c)
        {
            if (!CanConvertPossessiveArticle(c))
                c.RejectAccepted();
            else
                c.PushAccepted(); 
            
            return base.OnArticle(c);
        }

        public override IState OnConjunction(IContext c)
        {
            // His and her
            if( c.Accepted.Last().PartOfSpeech.Specific == "APPGE" &&  c.Current.IsCoordinatingConjunction() )
                return xSimpleList;

            return Reject;
        }
        
        public override IState OnDeterminer(IContext c)
        {
            if (c.Current.IsPostDeterminer())
                return Determiner;

            // Possibly a adjective or noun?

            c.RejectAccepted();
            return base.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            c.RejectAccepted();
            return base.OnExistential(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            return Reject;
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            return Reject;
        }

        public override IState OnInterjection(IContext c)
        {
            c.RejectAccepted();
            return base.OnInterjection(c);
        }

        public override IState OnNot(IContext c)
        {
            c.RejectAccepted();
            return base.OnNot(c);
        }

        public override IState OnPronoun(IContext c)
        {
            c.RejectAccepted();
            return base.OnPronoun(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            if (IgnorablePunctuation(c))
                return xIgnore;

            if (!CanConvertPossessiveArticle(c))
                c.RejectAccepted();
            else
                c.PushAccepted();

            return Reject;
        }

        public override IState OnUnclassified(IContext c)
        {
            return Reject;
        }

        public override IState OnVerb(IContext c)
        {
            // Very likely this is really an adjective or noun
            return ChooseVerbPsuedoState(c);
        }

        #endregion
    }
}
