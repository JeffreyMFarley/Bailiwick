using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class Decimal : Number
    {
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
            return Unknown;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            // Event transition
            if (allowedSuffix.Contains(c.Character))
                return SuffixedNumber;

            if( c.Character == ',')
                return Comma;

            if (c.Character == '.')
                return EndOfSentence;

            if (hyphens.Contains(c.Character))
                return xHyphen;

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        public override IParsingState OnSymbol(IContext c)
        {
            // Check for currency symbols
            if (currencySymbols.Contains(c.Character))
                return SuffixedNumber;

            // Event transition
            if (allowedSuffix.Contains(c.Character))
                return SuffixedNumber;

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }
    }
}
