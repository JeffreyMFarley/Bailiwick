using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Parsers.States
{
    internal class PsuedoApostrophe : ParsingStateBase
    {
        public override IParsingState OnLetter(IContext c)
        {
            return Contraction;
        }

        public override IParsingState OnDone(IContext c)
        {
            return Contraction;
        }

        public override IParsingState OnControl(IContext c)
        {
            c.ResolvePsuedoState(Contraction, true);

            return base.OnControl(c);
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            c.ResolvePsuedoState(Contraction, true);

            return base.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
            c.ResolvePsuedoState(Contraction, true);

            return base.OnSymbol(c);
        }
    }
}
