using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class PsuedoDateTime : ParsingStateBase
    {
        public override System.Collections.Generic.IEnumerable<Models.WordInstance> Extract(IContext c, string s)
        {
            yield break;
        }

        public override IParsingState OnControl(IContext c)
        {
            c.ResolvePsuedoState(Number, false);

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        public override IParsingState OnDigit(IContext c)
        {
            return Number;
        }

        public override IParsingState OnDone(IContext c)
        {
            c.ResolvePsuedoState(Number, false);

            return Trim;
        }

        public override IParsingState OnLetter(IContext c)
        {
            c.ResolvePsuedoState(Number, false);

            return Unknown;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            c.ResolvePsuedoState(Number, false);

            if (hyphens.Contains(c.Character))
                return xHyphen;

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }

        public override IParsingState OnSymbol(IContext c)
        {
            c.ResolvePsuedoState(Number, false);

            if (markers.Contains(c.Character))
                return Trim;

            return Unknown;
        }
    }
}
