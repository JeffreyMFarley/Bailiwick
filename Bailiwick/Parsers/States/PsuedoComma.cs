using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class PsuedoComma : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            return Comma.Extract(c, s);
        }

        public override IParsingState OnControl(IContext c)
        {
            c.ResolvePsuedoState(Number, false);
            return base.OnControl(c);
        }

        public override IParsingState OnDigit(IContext c)
        {
            if( c.Previous == Number )
                return Number;

            c.ResolvePsuedoState(Number, false);

            return Initial.OnDigit(c);
        }

        public override IParsingState OnDone(IContext c)
        {
            c.ResolvePsuedoState(Number, false);
            return this;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            if (c.Previous == Number)
                c.ResolvePsuedoState(Number, false);

            return base.OnPunctuation(c);
        }

    }
}
