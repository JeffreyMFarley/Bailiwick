using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class Number : ParsingStateBase
    {
        #region Properties

        readonly Regex yearTest = new Regex(@"^[12][0-9]{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected readonly char[] allowedSuffix = { '\'', '"', '%', '\u00b0', '\u2030', '\u2031', '\u2032', '\u2033'  };

        WordClass Cardinal
        {
            get
            {
                return cardinal ?? (cardinal = WordClasses.Number);
            }
        }
        WordClass cardinal;

        WordClass SingleYear
        {
            get
            {
                return singleYear ?? (singleYear = WordClasses.Specifics["NNT1"]);
            }
        }
        WordClass singleYear;

        #endregion

        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            if ( s.Length == 1 && s == "+" )
                yield return new WordInstance(s, WordClasses.CoordinatingConjunction);

            else if (s.Length == 1 && allowedSuffix.Contains(s[0]))
                yield break;

            else if (s.Length == 1 && currencySymbols.Contains(s[0]))
                yield break;

            else if (yearTest.IsMatch(s))
                yield return new WordInstance(s, SingleYear);

            else if( s.Last() == '.' ) // A trailing period most likely means 'full stop' not decimal point
            {
                var number = s.Substring(0, s.Length - 1);
                if( yearTest.IsMatch(number))
                    yield return new WordInstance(number, SingleYear);
                else
                    yield return new WordInstance(number, Cardinal);

                yield return new WordInstance(".", WordClasses.Period);
            }
            else
                yield return new WordInstance(s, Cardinal);
        }

        public override IParsingState OnDigit(IContext c)
        {
            return this;
        }

        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            return Ordinal;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            // Check for date or time markers
            if (dateTimeMarkers.Contains(c.Character))
                return xDateTime;

            // Event transition
            if (allowedSuffix.Contains(c.Character))
                return SuffixedNumber;

            // Transition
            switch (c.Character)
            {
                case '.': return Decimal;
                case ',': return xComma;
            }

            return base.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
            // Check for currency symbols
            if (currencySymbols.Contains(c.Character))
                return this;

            // Event transition
            if (allowedSuffix.Contains(c.Character))
                return SuffixedNumber;

            return base.OnSymbol(c);
        }
    }
}
