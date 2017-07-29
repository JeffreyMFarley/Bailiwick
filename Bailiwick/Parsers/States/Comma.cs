using Bailiwick.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class Comma : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            yield return new WordInstance(s, WordClasses.Comma);
        }

        public override IParsingState OnControl(IContext c)
        {
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
            return Initial.OnPunctuation(c);
        }

        public override IParsingState OnSymbol(IContext c)
        {
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
