using System;
using System.Collections.Generic;
using Bailiwick.Models;
using Esoteric.Collections;

namespace Bailiwick.Thoughts
{
    public class Engine : IContext
    {
        static public StringDistribution PredicateTally = new StringDistribution();

        #region Public Methods 

        public void Reset() 
        {
            Current = nullWord;
            buffer.Clear();
            state = StateFactory.Accumulate;
            isInterrogative = false;
            Flush();
        }

        public IEnumerable<Thought> Process(IEnumerable<WordInstance> words)
        {
            Reset();
            foreach(var w in words) {
                Current = w;
                state.OnWord(this, w);

                if (state.Ready)
                    yield return Extract();                   
            }

            Flush();
            if( buffer.Count > 0 )
                yield return Extract();
        }

        public void Flush()
        {
            buffer.AddRange(next);
            next.Clear();
        }

        public Thought Extract()
        {
            var result = new Thought(buffer.ToArray()) 
            {
                IsInterrogative = isInterrogative
            };
            Reset();
            return result;
        }

        #endregion
        #region IContext Members

        WordInstance Current { get; set; }

        IReadOnlyList<WordInstance> IContext.Buffer
        {
            get { return buffer; }
        }
        List<WordInstance> buffer = new List<WordInstance>();

        void IContext.Push(WordInstance w)
        {
            buffer.Add(w);
        }

        WordInstance IContext.Pop()
        {
            var last = buffer.Count - 1;
            var pop = buffer[last];
            buffer.RemoveAt(last);
            return pop;
        }

        WordInstance IContext.Peek
        {
            get 
            {
                if (buffer.Count == 0)
                    return nullWord;
                return buffer[buffer.Count - 1];
            }
        }
        WordInstance nullWord = new WordInstance("", WordClasses.BeforeClauseMarker);

        Queue<WordInstance> IContext.Next
        {
            get { return next; }
        }
        Queue<WordInstance> next = new Queue<WordInstance>();

        IState IContext.Status
        {
            get { return state; }
        }
        IState state;

        bool IContext.IsInterrogative 
        {
            get { return isInterrogative; }
            set { isInterrogative = value; } 
        }
        bool isInterrogative = false;

        void IContext.Transfer(IState newState)
        {
            state = newState;
            newState.Enter(this, Current);
        }

        #endregion
    }
}
