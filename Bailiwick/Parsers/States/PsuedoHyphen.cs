using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class PsuedoHyphen : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            yield return new WordInstance("-", WordClasses.Hyphen);
        }

        public override IParsingState OnControl(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return base.OnControl(c);
        }

        public override IParsingState OnDigit(IContext c)
        {
            if (c.Previous == this)
            {
                c.PushHistory();
                return Initial.OnDigit(c);
            }

            if (c.Previous == Number 
             || c.Previous == Ordinal 
             || c.Previous == Decimal
             || c.Previous == SuffixedNumber)
            {
                return NumberRange;
            }

            c.ResolvePsuedoState(c.Previous, false);
            return Initial.OnDigit(c);
        }

        public override IParsingState OnDone(IContext c)
        {
            if( c.Previous != this )
                c.ResolvePsuedoState(c.Previous, false);

            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            if (c.Previous == this)
            {
                c.PushHistory();
                return Initial.OnLetter(c);
            }

            if (c.Previous != Word && c.Previous != Contraction)
            {
                c.ResolvePsuedoState(c.Previous, false);
                c.PushHistory();
                return Initial.OnLetter(c);
            }

            // em-dashes between words count as separators
            if (c.PreviousCharacter == '\u2014')
            {
                c.ResolvePsuedoState(c.Previous, false);
                c.PushHistory();
                return Initial.OnLetter(c);
            }

            return c.Previous;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);

            if (hyphens.Contains(c.Character))
                return this;

            return base.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
            c.ResolvePsuedoState(c.Previous, false);
            return Initial.OnSymbol(c);
        }
    }
}
