using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers
{
    public class Context : IContext
    {
        #region Public Properties

        public char Character { get; private set; }
        public char PreviousCharacter { get; private set; }

        public IParsingState State
        {
            get { return _state; }
            set 
            {
                bool same = value.Equals(_state);
                if (!same)
                {
                    _state.OnExit(this);
                    value.OnEntry(this);
                }

                Previous = _state;
                _state = value; 
            }
        }
        private IParsingState _state = ParsingStateBase.Initial;

        public IParsingState Previous
        {
            get { return _previous; }
            private set { _previous = value; }
        }
        private IParsingState _previous = ParsingStateBase.Initial;

        public ICorpus Corpus { get; internal set; }

        #endregion

        #region Private Members

        struct StackFrame
        {
            internal IParsingState State;
            internal int  Position;
        }

        private int Position { get; set; }

        private static int MAX_HISTORY = 25;
        private StackFrame[] _stack = new StackFrame[MAX_HISTORY];
        private int Depth { get; set; }

        private void Reset()
        {
            Character = '\0';
            PreviousCharacter = '\0';
            Position = 0;
            State = ParsingStateBase.Initial;

            Depth = 0;
        }

        #endregion

        public IEnumerable<WordInstance> Parse(string s)
        {
             if (string.IsNullOrEmpty(s))
                yield break;

            // FYI - Regular Expressions add orders of magnitude to the run length of this function!
            //if (IpAddress.IsMatch(s))
            //{
            //    yield return new WordInstance(s);
            //    yield break;
            //}

            if (s.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                yield return new WordInstance(s, WordClasses.Noun);
                yield break;
            }

            Reset();

            // Run the characters through the States
            for (Position = 0; Position < s.Length; Position++)
            {
                PreviousCharacter = Character;
                Character = s[Position];

                // Trigger the proper event
                if (char.IsLetter(Character)) 
                    State = State.OnLetter(this);
                else if (char.IsDigit(Character))
                    State = State.OnDigit(this);
                else if (char.IsPunctuation(Character))
                    State = State.OnPunctuation(this);
                else if (char.IsSymbol(Character))
                    State = State.OnSymbol(this);
                else if (char.IsControl(Character))
                    State = State.OnControl(this);
            }

            // Run through the last State
            PreviousCharacter = Character;
            State = State.OnDone(this);
            PushHistory();

            // Pop and return
            int start = 0;
            int end = 0;
            string s1;
            for (int i = 0; i < Depth; i++)
            {
                end = _stack[i].Position;

                if ( start != end )
                {
                    s1 = s.Substring(start, end - start);

                    foreach (var w in _stack[i].State.Extract(this, s1))
                        yield return w;
                }

                start = end;
            }
        }
    
        public void PushHistory()
        {
            if( Depth == MAX_HISTORY - 1 )
                throw new IndexOutOfRangeException("Too much history");

            _stack[Depth++] = new StackFrame{ State = State, Position = Position };
        }

        public void ResolvePsuedoState(IParsingState state, bool includeLastCharacterInHistory)
        {
            if (Depth == MAX_HISTORY - 1)
                throw new IndexOutOfRangeException("Too much history");

            int position = (includeLastCharacterInHistory) ? Position : Position - 1;
            _stack[Depth++] = new StackFrame { State = state, Position = position };
        }
    }
}
