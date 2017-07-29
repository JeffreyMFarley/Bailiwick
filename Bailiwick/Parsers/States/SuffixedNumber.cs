using System;
using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class SuffixedNumber : Number
    {
        public override IParsingState OnDigit(IContext c)
        {
            return Unknown;
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
            // Only allow a second '
            if( c.Character == '\'' )
                return this;

            if (c.Character == ',')
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
            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }
    }
}
