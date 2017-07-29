using Bailiwick.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class Trim : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            if (s == ";")
                yield return new WordInstance(s, WordClasses.Semicolon);

            if (singleQuotes.Contains(s))
                yield return new WordInstance(s, WordClasses.SingleQuote);

            if( doubleQuotes.Contains(s) )
                yield return new WordInstance(s, WordClasses.DoubleQuote);

            yield break;
        }

        public override IParsingState OnControl(IContext c)
        {
            if (markers.Contains(c.Character))
                return this;

            return Initial.OnControl(c);
        }

        public override IParsingState OnDigit(IContext c)
        {
            return Initial.OnDigit(c);
        }

        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            return Initial.OnLetter(c);
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            if (hyphens.Contains(c.Character))
                return xHyphen;

            if (markers.Contains(c.Character))
                return this;

            return Initial.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
            if (markers.Contains(c.Character))
                return this;

            return Initial.OnSymbol(c);
        }

        public override void OnEntry(IContext c)
        {
            c.PushHistory();
        }

        public override void OnExit(IContext c)
        {
            c.PushHistory();
        }
    }
}
