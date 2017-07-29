using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders
{
    public interface IState
    {
        // Effects
        void OnEntry(IContext c);
        void OnExit(IContext c);

        // Events
        IState OnAdjective(IContext c);
        IState OnAdposition(IContext c);
        IState OnAdverb(IContext c);
        IState OnArticle(IContext c);
        IState OnConjunction(IContext c);
        IState OnDeterminer(IContext c);
        IState OnExistential(IContext c);
        IState OnGenitiveMarker(IContext c);
        IState OnInfinitiveMarker(IContext c);
        IState OnInterjection(IContext c);
        IState OnLetter(IContext c);
        IState OnNot(IContext c);
        IState OnNoun(IContext c);
        IState OnNumber(IContext c);
        IState OnPronoun(IContext c);
        IState OnPunctuation(IContext c);
        IState OnUnclassified(IContext c);
        IState OnVerb(IContext c);

        // Queries
        PhraseBuilderStatus OverallStatus { get; }
        WordClassType GeneralWordClass { get; }

        // Methods
        IPhrase Build(IContext c);
    }
}
