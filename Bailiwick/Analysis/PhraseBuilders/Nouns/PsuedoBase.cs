using System;
using System.Diagnostics;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns
{
    /// <summary>
    /// The base class for psuedostates
    /// </summary>
    abstract internal class PsuedoBase : IState
    {
        #region Effects

        abstract public void OnEntry(IContext c);

        virtual public void OnExit(IContext c) { }

        #endregion

        #region IState Members

        virtual public IState OnAdjective(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnAdjective(c);
        }

        virtual public IState OnAdposition(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnAdposition(c);
        }

        virtual public IState OnAdverb(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnAdverb(c);
        }

        virtual public IState OnArticle(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnArticle(c);
        }

        virtual public IState OnConjunction(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnConjunction(c);
        }

        virtual public IState OnDeterminer(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnDeterminer(c);
        }

        virtual public IState OnExistential(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnExistential(c);
        }

        virtual public IState OnGenitiveMarker(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnGenitiveMarker(c);
        }

        virtual public IState OnInfinitiveMarker(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnInfinitiveMarker(c);
        }

        virtual public IState OnInterjection(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnInterjection(c);
        }

        virtual public IState OnLetter(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnLetter(c);
        }

        virtual public IState OnNot(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnNot(c);
        }

        virtual public IState OnNoun(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnNoun(c);
        }

        virtual public IState OnNumber(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnNumber(c);
        }

        virtual public IState OnPronoun(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnPronoun(c);
        }

        virtual public IState OnPunctuation(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnPunctuation(c);
        }

        virtual public IState OnUnclassified(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnUnclassified(c);
        }

        virtual public IState OnVerb(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return BaseState.Initial.OnVerb(c);
        }

        virtual public PhraseBuilderStatus OverallStatus
        {
            get { return PhraseBuilderStatus.Building; }
        }

        abstract public WordClassType GeneralWordClass { get; }

        virtual public IPhrase Build(IContext c)
        {
            Console.WriteLine("Unexpected call to PsuedoBase::Build");
            return BaseState.InternalBuild(c);
        }

        #endregion
    }
}
