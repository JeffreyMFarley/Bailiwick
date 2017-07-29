using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class NO : Initial
    {
        public override void OnEntry(IContext c)
        {
            c.BufferCurrent();
        }

        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Article; }
        }

        public override PhraseBuilderStatus OverallStatus
        {
            get
            {
                return PhraseBuilderStatus.Building;
            }
        }

        public override IPhrase Build(IContext c)
        {
            var last = c.Accepted.Last();
            if (last.GeneralWordClass == WordClassType.Adjective)
                return new AdjectivePhrase(c.Accepted);

            if( last.GeneralWordClass == WordClassType.Interjection )
                return new InterjectionPhrase(c.Accepted);

            return InternalBuild(c);
        }

        void ResolvePsuedoState(IContext c)
        {
            var last = c.Buffer.Last();
            var nos = c.Buffer.Where(x => x.Lemma == "no");

            if (c.Buffer.Count == 1 )
            {
                // Convert "no" to Interjection
                foreach (var n in nos)
                    n.PartOfSpeech = WordClasses.Interjection;

                c.AcceptBuffer();
            }
            else if (last.IsPotentialAdjective() || c.Buffer.Any(x => x.IsPotentialAdjective()))
            {
                 // Convert "no" to adverb
                foreach (var n in nos)
                    c.ForceConvert.ToAdverb(n);

                c.ResolvePsuedoState(c.Previous, false);                
            }
            else if (last.GeneralWordClass == WordClassType.Adjective && c.Current.GeneralWordClass == WordClassType.Verb)
            {
                // Convert eveything to adverbs
                foreach (var n in c.Buffer)
                    c.ForceConvert.ToAdverb(n);

                c.ResolvePsuedoState(c.Previous, false);
            }
            else if (last.GeneralWordClass == WordClassType.Adjective)
            {
                // Convert "no" to adverb
                foreach (var n in nos)
                    c.ForceConvert.ToAdverb(n);

                c.AcceptBuffer();
            }
            else
                c.AcceptBuffer();
        }

        void ProcessAsArticle(IContext c)
        {
            var gerunds = c.Buffer.Where(x => x.IsPotentialAdjective());

            // Convert any gerunds to adjectives
            foreach (var g in gerunds)
                c.ForceConvert.ToAdjective(g);

            c.AcceptBuffer();
        }

        #region Event Handlers

        public override IState OnAdposition(IContext c)
        {
            ResolvePsuedoState(c);
            return base.OnAdposition(c);
        }

        public override IState OnAdjective(IContext c)
        {
            // The adjective is ambiguous
            c.BufferCurrent();
            return this;
        }

        public override IState OnAdverb(IContext c)
        {
            // The adverb is ambiguous
            c.BufferCurrent();
            return this;
        }

        public override IState OnArticle(IContext c)
        {
            ResolvePsuedoState(c);
            return base.OnArticle(c);
        }

        public override IState OnConjunction(IContext c)
        {
            ResolvePsuedoState(c);
            return RejectAndDone;
        }

        public override IState OnDeterminer(IContext c)
        {
            if (c.Current.IsPostDeterminer())
                ProcessAsArticle(c);
            else
                ResolvePsuedoState(c);

            return base.OnDeterminer(c);
        }

        public override IState OnExistential(IContext c)
        {
            ResolvePsuedoState(c);
            return base.OnExistential(c);
        }

        public override IState OnGenitiveMarker(IContext c)
        {
            ResolvePsuedoState(c);
            return RejectAndDone;
        }

        public override IState OnLetter(IContext c)
        {
            ProcessAsArticle(c);
            return Letter;
        }

        public override IState OnInfinitiveMarker(IContext c)
        {
            ResolvePsuedoState(c);
            return Reject;
        }

        public override IState OnInterjection(IContext c)
        {
            ResolvePsuedoState(c);
            return base.OnInterjection(c);
        }

        public override IState OnNot(IContext c)
        {
            ResolvePsuedoState(c);
            return base.OnNot(c);
        }

        public override IState OnNoun(IContext c)
        {
            ProcessAsArticle(c);
            return Noun;
        }

        public override IState OnNumber(IContext c)
        {
            ProcessAsArticle(c);
            return base.OnNumber(c);
        }

        public override IState OnPronoun(IContext c)
        {
            if (c.Current.Normalized == "one")
                ProcessAsArticle(c);
            else
                ResolvePsuedoState(c);

            return base.OnPronoun(c);
        }

        public override IState OnPunctuation(IContext c)
        {
            if (IgnorablePunctuation(c))
                return xIgnore;

            ResolvePsuedoState(c);
            return RejectAndDone;
        }

        public override IState OnUnclassified(IContext c)
        {
            ResolvePsuedoState(c);
            return RejectAndDone;
        }

        public override IState OnVerb(IContext c)
        {
            // The participle is ambiguous
            if (c.Current.IsPotentialAdjective())
            {
                c.BufferCurrent();
                return this;
            }

            ResolvePsuedoState(c);
            return RejectAndDone;
        }

        #endregion
    }
}
