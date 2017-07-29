using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers
{
    public abstract class ParsingStateBase : IParsingState, IEquatable<IParsingState>
    {
        protected readonly char[] separators = { '\r', '\n', '\u0085', '\u2028', '\u2029' };
        protected readonly char[] apostrophes = { '\'', '\u2019' };
        protected readonly char[] currencySymbols = { '$', '\u00a2', '\u00a3', '\u00a4', '\u00a5', '\u20ac' };
        protected readonly char[] hyphens = { '-', '\u2012', '\u2013', '\u2014', '\u2015' };
        protected readonly char[] markers = 
        { 
            '[', ']', '(', ')', '{', '}', '⟨', '⟩', ':', ';', '~', '@', '#', '^', '*', '=', '|'
            ,'\u2026' //ellipsis
            ,'`', '"', '\u2018', '\u201b', '\u201c', '\u201d', '\u201f' // quotation marks
            ,'/', '\\'
            , '\u2022', '\u2023', '\u25e6', '\u2043', '\u2219' // bullets
            , '\u2033' //double-prime or ditto
            , '§', '¶', '·', '‖', '‗', '†', '‡', '\u2024', '\u2025'  // other Unicode punctuation symbols
        };
        protected readonly char[] dateTimeMarkers = { '\\', '/', ':' };

        protected readonly string[] doubleQuotes = { "\"", "\u201c", "\u201d", "\u201f" };
        protected readonly string[] singleQuotes = { "\'", "\u2018", "\u2019", "\u201b" };

        #region Instances

        static public readonly IParsingState Unknown = new States.Unknown();
        static public readonly IParsingState Initial = new States.Initial();

        static protected readonly IParsingState Trim = new States.Trim();

        static protected readonly IParsingState Word = new States.Word();
        static protected readonly IParsingState Number = new States.Number();

        static protected readonly IParsingState Comma = new States.Comma();
        static protected readonly IParsingState EndOfSentence = new States.EndOfSentence();
              
        static protected readonly IParsingState Contraction = new States.Contraction();

        static protected readonly IParsingState Ordinal = new States.Ordinal();
        static protected readonly IParsingState Decimal = new States.Decimal();
        static protected readonly IParsingState SuffixedNumber = new States.SuffixedNumber();
        static protected readonly IParsingState NumberRange = new States.NumberRange();

        static protected readonly IParsingState xApostrophe = new States.PsuedoApostrophe();
        static protected readonly IParsingState xComma = new States.PsuedoComma();
        static protected readonly IParsingState xDateTime = new States.PsuedoDateTime();
        static protected readonly IParsingState xHyphen = new States.PsuedoHyphen();
        static protected readonly IParsingState xPeriod = new States.PsuedoPeriod();

        #endregion

        #region IParsingState Members

        virtual public void OnEntry(IContext c)
        {
        }

        virtual public void OnExit(IContext c)
        {
        }

        virtual public IParsingState OnControl(IContext c)
        {
            if ( separators.Contains(c.Character) )
                return EndOfSentence;

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        virtual public IParsingState OnDigit(IContext c)
        {
            return Unknown;
        }

        virtual public IParsingState OnLetter(IContext c)
        {
            return Unknown;
        }

        virtual public IParsingState OnPunctuation(IContext c)
        {
            if (c.Character == '!' || c.Character == '?')
                return EndOfSentence;

            if( c.Character == ',' )
                return Comma;

            if (apostrophes.Contains(c.Character))
                return Trim;

            if( hyphens.Contains(c.Character))
                return xHyphen;

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        virtual public IParsingState OnSymbol(IContext c)
        {
            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        virtual public IParsingState OnDone(IContext c)
        {
            return Unknown;
        }

        virtual public IEnumerable<WordInstance> Extract(IContext c, string s) 
        { 
            if( s.Length == 1 )
            {
                if( s == "&" )
                    yield return new WordInstance(s, WordClasses.CoordinatingConjunction);
            
                else if( s == ".")
                    yield return new WordInstance(s, WordClasses.Period);

                else
                    yield return new WordInstance(s); 
            }
            else
                yield return new WordInstance(s); 
        }

        #endregion

        #region IEquatable<IParsingState> Members

        public bool Equals(IParsingState other)
        {
            if( other == null )
                return false;

            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IParsingState);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        #endregion
    }
}
