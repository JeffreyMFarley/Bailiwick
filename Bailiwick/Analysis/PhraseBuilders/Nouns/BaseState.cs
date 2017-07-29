using Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns
{
    abstract public class BaseState : IState
    {
        #region State Instances
        
        static internal IState Initial = new Initial();
        static internal IState Reject = new Reject();
        static internal IState AcceptAndDone = new AcceptAndDone();
        static internal IState RejectAndDone = new RejectAndDone();

        static internal IState Adjective = new Adjective();
        static internal IState Adposition = new Adposition();
        static IState Adverb = new Adverb();
        static IState Article = new Article();
        static internal IState BeforeDeterminer = new BeforeDeterminer();
        static internal IState Determiner = new Determiner();
        static internal IState Existential = new Existential();
        static internal IState Genitive = new Genitive();
        static internal IState Interjection = new Interjection();
        static internal IState Letter = new Letter();
        static internal IState Not = new Not();
        static internal IState Noun = new Noun();
        static internal IState Number = new Number();
        static internal IState Pronoun = new Pronoun();

        static IState ArticleNO = new NO();

        static internal IState xConjunction = new PsuedoConjunction();
        static internal IState xGerund = new PsuedoGerund();
        static internal IState xInfinitive = new PsuedoInfinitive();
        static internal IState xIgnore = new PsuedoIgnore();
        static internal IState xSimpleList = new PsuedoSimpleList();

        #endregion

        #region Punctuation Members
        
        static readonly string[] ignorablePunctuation = 
        { 
            "[", "]", "(", ")", "{", "}", "⟨", "⟩", "~", "@", "#", "^", "*", "=", "|"
            ,"`", "\"", "\u2018", "\u201b", "\u201c", "\u201d", "\u201f" // quotation marks
            ,"/", "\\"
            , "\u2022", "\u2023", "\u25e6", "\u2043", "\u2219" // bullets
            , "\u2033" //double-prime or ditto
            , "§", "¶", "·", "‖", "‗", "†", "‡", "\u2024", "\u2025"  // other Unicode punctuation symbols
        };

        static internal bool IgnorablePunctuation(IContext c)
        {
            return ignorablePunctuation.Contains(c.Current.Normalized);
        }

        static readonly string[] endingPunctuation = 
        { 
            "!", "?", "."
            , ":", ";", ","
            ,"\u2012", "\u2013", "\u2014", "\u2015"    // dashes
            ,"\u2026" //ellipsis
        };

        static internal bool IsEndingPunctuation(IContext c)
        {
            return endingPunctuation.Contains(c.Current.Normalized);
        }

        #endregion

        #region Effects

        virtual public void OnEntry(IContext c)
        {
            c.AcceptCurrent();
        }

        virtual public void OnExit(IContext c)
        {
        }

        #endregion

        #region IState Members

        virtual public IState OnAdjective(IContext c)
        {
            return Adjective;
        }

        virtual public IState OnAdposition(IContext c)
        {
            c.PushAccepted();
            return Initial.OnAdposition(c);
        }

        virtual public IState OnAdverb(IContext c)
        {
            return ChooseAdverbState(c);
        }

        virtual public IState OnArticle(IContext c)
        {
            c.PushAccepted();
            return Initial.OnArticle(c);
        }

        virtual public IState OnConjunction(IContext c)
        {
            if (c.Current.IsCoordinatingConjunction())
                return xSimpleList;

            return RejectAndDone;
        }

        virtual public IState OnDeterminer(IContext c)
        {
            c.PushAccepted();
            return Initial.OnDeterminer(c);
        }

        virtual public IState OnExistential(IContext c)
        {
            c.PushAccepted();
            return Initial.OnExistential(c);
        }

        virtual public IState OnGenitiveMarker(IContext c)
        {
            return RejectAndDone;
        }

        virtual public IState OnInfinitiveMarker(IContext c)
        {
            return RejectAndDone;
        }

        virtual public IState OnInterjection(IContext c)
        {
            c.PushAccepted();
            return Initial.OnInterjection(c);
        }

        virtual public IState OnLetter(IContext c)
        {
            return Letter;
        }

        virtual public IState OnNot(IContext c)
        {
            c.PushAccepted();
            return Initial.OnNot(c);
        }

        virtual public IState OnNoun(IContext c)
        {
            return Noun;
        }

        virtual public IState OnNumber(IContext c)
        {
            return Number;
        }

        virtual public IState OnPronoun(IContext c)
        {
            return Pronoun;
        }

        virtual public IState OnPunctuation(IContext c)
        {
            return ChoosePunctuationState(c, RejectAndDone);
        }

        virtual public IState OnUnclassified(IContext c)
        {
            return RejectAndDone;
        }

        virtual public IState OnVerb(IContext c)
        {
            return RejectAndDone;
        }

        virtual public PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Building; }
        }

        abstract public WordClassType GeneralWordClass { get; }

        virtual public IPhrase Build(IContext c)
        {
            return InternalBuild(c);
        }

        #endregion

        #region Substate methods

        static internal IState ChooseAdverbState(IContext c)
        {
            return Adverb;
        }

        static internal IState ChooseArticleState(IContext c)
        {
            return (c.Current.Lemma == "no") ? ArticleNO : Article;
        }

        static internal IState ChooseDeterminerState(IContext c)
        {
            return (c.Current.IsPreDeterminer()) ? BeforeDeterminer : Determiner;
        }

        static internal IState ChoosePunctuationState(IContext c, IState defaultResult)
        {
            if ( IgnorablePunctuation(c) )
                return xIgnore;

            return defaultResult;
        }

        static internal IState ChooseVerbPsuedoState(IContext c)
        {
            if( c.Current.IsPotentialAdjective() )
                return xGerund;

            return RejectAndDone;
        }

        #endregion

        static internal IPhrase InternalBuild(IContext c)
        {
            var result = new NounPhrase();

            foreach (var w in c.Accepted)
                result.AddTail(w);

            return (result.Head != null) ? result : null;
        }
    }
}
