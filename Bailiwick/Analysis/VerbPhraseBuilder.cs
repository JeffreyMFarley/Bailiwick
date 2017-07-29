using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using Bailiwick.Analysis.PhraseBuilders;

namespace Bailiwick.Analysis
{
    public class VerbPhraseBuilder : PhraseBuilders.IPhraseBuilder
    {
        public PhraseBuilderStatus State { get; set; }

        public List<IPhrase> Phrases
        {
            get { return phrases ?? (phrases = new List<IPhrase>()); }
        }
        List<IPhrase> phrases;

        VerbPhrase Current { get; set; }

        public void Reset()
        {
            State = PhraseBuilderStatus.Standby;
            Phrases.Clear();
            Current = null;
        }

        public PhraseBuilderStatus Process(WordInstance word)
        {
            if (word == null)
                OnDone();

            else if (!word.IsVerbPhrasePart())
                OnDone();

            else if (Current != null)
            {
                if (Current.CanAcceptAsNextWord(word))
                {
                    Current.AddTail(word);
                }
                else
                {
                    StartNew();
                    Current.AddTail(word);
                }
            }

            else if (State == PhraseBuilderStatus.Standby)
            {
                StartNew();
                Current.AddTail(word);
            }

            else
            {
                Current.AddTail(word);
            }

            return State;
        }

        public void Finished()
        {
            OnDone();
        }

        void StartNew()
        {
            State = PhraseBuilderStatus.Building;

            if (Current != null && Current.Head != null)
                Phrases.Add(Current);

            Current = new VerbPhrase();
        }

        void OnDone()
        {
            if (State == PhraseBuilderStatus.Standby)
                return;

            State = PhraseBuilderStatus.Done;

            if (Current != null && Current.Head != null)
                Phrases.Add(Current);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", State, (Current == null) ? "null" : Current.ToString(), Phrases.Count);
        }
    }
}
