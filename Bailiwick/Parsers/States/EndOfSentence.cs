using Bailiwick.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Parsers.States
{
    internal class EndOfSentence : ParsingStateBase
    {
        public override IEnumerable<WordInstance> Extract(IContext ctxt, string s)
        {
            foreach (var c in s.Replace("\r", "\n").Replace("\n\n", "\n"))
            {
                var wi = new WordInstance(c.ToString());

                switch (c)
                {
                    case '!': 
                        wi.PartOfSpeech = WordClasses.Exclamation; 
                        break;

                    case '?':
                        wi.PartOfSpeech = WordClasses.Question;
                        break;

                    case '.':
                        wi.PartOfSpeech = WordClasses.Period;
                        break;

                    case '\n':
                    case '\u2028':
                    case '\u2029':
                        wi.PartOfSpeech = WordClasses.NewLine;
                        break;

                    default:
                        wi.PartOfSpeech = WordClasses.Punctuation;
                        break;
                }

                yield return wi;
            }
        }

        public override IParsingState OnControl(IContext c)
        {
            if (separators.Contains(c.Character))
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
            if (c.Character == '!' || c.Character == '?')
                return this;

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
