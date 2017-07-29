using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Parsers.States
{
    internal class Initial : ParsingStateBase
    {
        public override IParsingState OnDigit(IContext c)
        {
            return Number;
        }

        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            return Word;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            return base.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
            if (currencySymbols.Contains(c.Character))
                return Number;

            // Check for certain symbols
            switch (c.Character)
            {
                case '+': return Number;
            }

            return base.OnSymbol(c);
        }
    }
}
