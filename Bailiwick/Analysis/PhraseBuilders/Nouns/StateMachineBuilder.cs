using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns
{
    public class StateMachineBuilder : IContext, IPhraseBuilder
    {
        public StateMachineBuilder(IConvert tryConvert, IConvert forceConvert, IPhraseBuilder nextBuilder)
        {
            TryConvert = tryConvert;
            ForceConvert = forceConvert;
            NextBuilder = nextBuilder;

            TheContext = this; 
            //TheContext = new TracingContext(this);
        }

        IContext TheContext { get; set; }

        #region IContext Properties

        public IState State
        {
            get { return _state; }
            set
            {
                bool same = value.Equals(_state);
                if (!same)
                {
                    _state.OnExit(TheContext);
                    value.OnEntry(TheContext);
                }

                Previous = _state;
                _state = value;
            }
        }
        private IState _state = BaseState.Initial;

        public IState Previous
        {
            get { return _previous; }
            private set { _previous = value; }
        }
        private IState _previous = BaseState.Initial;

        public List<WordInstance> Accepted
        {
            get { return accepted ?? (accepted = new List<WordInstance>()); }
        }
        List<WordInstance> accepted;

        public List<WordInstance> Buffer
        {
            get { return buffer ?? (buffer = new List<WordInstance>()); }
        }
        List<WordInstance> buffer;

        public WordInstance Current { get; private set; }
        public IConvert TryConvert { get; private set; }
        public IConvert ForceConvert { get; private set; }

        #endregion

        #region IContext methods

        public void ResolvePsuedoState(IState state, bool includeBuffer)
        {
            if (HasRouteToNext())
            {
                RouteToNext();
                return;
            } 

            if (includeBuffer)
            {
                AddBufferToAccepted();
                NextBuilder.Finished();
            }
            else
            {
                foreach (var w in Buffer)
                    NextBuilder.Process(w);
            }

            ExtractPhrase(state);
        }

        public void AcceptCurrent()
        {
            AddBufferToAccepted();
            AddToAccepted(Current);

            if( !HasRouteToNext() )
                NextBuilder.Finished();
        }

        public void BufferCurrent()
        {
            IsBufferAllAdverbs = (Current.GeneralWordClass == WordClassType.Adverb) & (IsBufferAllAdverbs ?? true);
            IsBufferAllVerbParts = Current.IsVerbPhrasePart() & (IsBufferAllVerbParts ?? true);
            DoesBufferHaveVerb = (Current.GeneralWordClass == WordClassType.Verb) | (DoesBufferHaveVerb ?? false);

            Buffer.Add(Current);
        }

        public void RejectCurrent()
        {
            foreach (var w in Buffer)
                NextBuilder.Process(w);
            NextBuilder.Process(Current);

            ResetBuffer();
        }

        public void PushAccepted()
        {
            if (HasRouteToNext())
            {
                RouteToNext();
                return;
            } 
            
            ExtractPhrase(State);

            NextBuilder.Finished();
        }

        public void RejectAccepted()
        {
            foreach (var w in Accepted)
                NextBuilder.Process(w);
            foreach (var w in Buffer)
                NextBuilder.Process(w);
            NextBuilder.Process(Current);

            ResetAccepted();
            ResetBuffer();
        }

        public void AcceptBuffer()
        {
            AddBufferToAccepted();

            if (!HasRouteToNext())
                NextBuilder.Finished();
        }

        void AddToAccepted(WordInstance w)
        {
            IsAcceptedAllAdverbs = (w.GeneralWordClass == WordClassType.Adverb) & (IsAcceptedAllAdverbs ?? true);
            IsAcceptedAllVerbParts = w.IsVerbPhrasePart() & (IsAcceptedAllVerbParts ?? true);
            Accepted.Add(w);
        }

        void AddBufferToAccepted()
        {
            if (Buffer.Count > 0)
            {
                foreach (var w in Buffer)
                    AddToAccepted(w);
                ResetBuffer();
            }
        }

        #endregion

        #region IPhraseBuilder Members

        PhraseBuilderStatus IPhraseBuilder.State
        {
            get { return State.OverallStatus; }
        }

        public List<IPhrase> Phrases
        {
            get { return phrases ?? (phrases = new List<IPhrase>()); }
        }
        List<IPhrase> phrases;

        public void Reset()
        {
            ResetMachine();
            Phrases.Clear();
        }

        public PhraseBuilderStatus Process(WordInstance word)
        {
            Current = word;

            // Call the proper event
            IContext c = TheContext;
            switch (word.GeneralWordClass)
            {
                #region Big Switch
                case WordClassType.Adjective:
                    State = State.OnAdjective(c);
                    break;

                case WordClassType.Adposition:
                    State = State.OnAdposition(c);
                    break;

                case WordClassType.Adverb:
                    State = State.OnAdverb(c);
                    break;

                case WordClassType.Article:
                    State = State.OnArticle(c);
                    break;

                case WordClassType.Conjunction:
                    State = State.OnConjunction(c);
                    break;

                case WordClassType.Determiner:
                    State = State.OnDeterminer(c);
                    break;

                case WordClassType.Existential:
                    State = State.OnExistential(c);
                    break;

                case WordClassType.GenitiveMarker:
                    State = State.OnGenitiveMarker(c);
                    break;

                case WordClassType.InfinitiveMarker:
                    State = State.OnInfinitiveMarker(c);
                    break;

                case WordClassType.Interjection:
                    State = State.OnInterjection(c);
                    break;

                case WordClassType.Letter:
                    State = State.OnLetter(c);
                    break;

                case WordClassType.Not:
                    State = State.OnNot(c);
                    break;

                case WordClassType.Noun:
                    State = State.OnNoun(c);
                    break;

                case WordClassType.Number:
                    State = State.OnNumber(c);
                    break;

                case WordClassType.Pronoun:
                    State = State.OnPronoun(c);
                    break;

                case WordClassType.Punctuation:
                    State = State.OnPunctuation(c);
                    break;

                case WordClassType.Unclassified:
                    State = State.OnUnclassified(c);
                    break;

                case WordClassType.Verb:
                    State = State.OnVerb(c);
                    break;
                #endregion
            }

            var result = State.OverallStatus;
            if (result == PhraseBuilderStatus.Done)
                ExtractPhrase(State);

            return result;
        }

        public void Finished()
        {
            ExtractPhrase(State);
        }

        #endregion

        void ResetMachine()
        {
            ResetAccepted();
            ResetBuffer();

            State = BaseState.Initial;
            Previous = BaseState.Initial;
        }

        void ExtractPhrase(IState state)
        {
            if (Accepted.Count > 0)
            {
                var p = state.Build(TheContext);
                if (p != null)
                    Phrases.Add(p);

                if (TheContext != this)
                {
                    var s = string.Join(" ", Accepted);
                    Console.Write("Extracting ");
                    if (p != null)
                        Console.Write(string.Format("{0} ", p.GetType().Name));
                    else
                        Console.Write("<failed> ");
                    Console.WriteLine(s);
                }
            }

            ResetMachine();
        }

        #region Next Builder members

        IPhraseBuilder NextBuilder { get; set; }

        bool? IsAcceptedAllAdverbs { get; set; }
        bool? IsBufferAllAdverbs { get; set; }
        bool? IsAcceptedAllVerbParts { get; set; }
        bool? IsBufferAllVerbParts { get; set; }
        bool? DoesBufferHaveVerb { get; set; }

        void ResetAccepted()
        {
            Accepted.Clear();
            IsAcceptedAllAdverbs = null;
            IsAcceptedAllVerbParts = null;
        }

        void ResetBuffer()
        {
            Buffer.Clear();
            IsBufferAllAdverbs = null;
            IsBufferAllVerbParts = null;
            DoesBufferHaveVerb = null;
        }

        bool HasRouteToNext()
        {
            // Pass the adverb to the verb phrase being built
            if (NextBuilder.State == PhraseBuilderStatus.Building && (IsAcceptedAllAdverbs ?? false))
                return true;

            // Everything in the bucket is a verb part, there is a route
            var allVerbParts = (IsAcceptedAllVerbParts ?? false) && (IsBufferAllVerbParts ?? true);
            if ( allVerbParts && !(IsAcceptedAllAdverbs ?? false))
                return true;

            // Maybe the start of a verb phrase
            if (NextBuilder.State == PhraseBuilderStatus.Standby && (IsAcceptedAllAdverbs ?? false) && (DoesBufferHaveVerb ?? false))
                return true;

            return false;
        }

        void RouteToNext()
        {
            if (TheContext != this)
            {
                var s = string.Join(" ", Accepted);
                Console.Write("Routing to Next ");
                Console.WriteLine(s);
            }

            foreach (var w in Accepted)
                NextBuilder.Process(w);
            foreach (var w in Buffer)
                NextBuilder.Process(w);

            ResetMachine();
        }

        #endregion

    }
}
