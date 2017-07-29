using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;
using Bailiwick.DAL;

namespace Bailiwick.Parsers.States
{
    internal class PsuedoPeriod : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            // Single letter, return as separate
            if (s.Length == 2) 
            { 
                yield return new WordInstance(s[0].ToString());
                yield return new WordInstance(".", WordClasses.Period);
                yield break;
            }

            // check for common abbreviations
            if (c.Corpus.Abbreviations.Keys.Contains(s, StringComparer.InvariantCultureIgnoreCase))
            {
                yield return new WordInstance(s, c.Corpus.Abbreviations[s.ToLower()]);
                yield break;
            }

            yield return new WordInstance(s.Substring(0, s.Length - 1));
            yield return new WordInstance(".", WordClasses.Period);
        }

        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            switch (c.Character)
            {
                case '.': return this;
            }

            return base.OnPunctuation(c);
        }

    }
}
