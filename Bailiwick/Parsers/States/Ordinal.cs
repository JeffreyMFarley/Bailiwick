using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class Ordinal : ParsingStateBase
    {
        #region Properties

        protected readonly Regex yearTest = new Regex(@"^[12][0-9]{3}s$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        WordClass PartOfSpeechOrdinal
        {
            get
            {
                return partOfSpeechOrdinal ?? (partOfSpeechOrdinal = WordClasses.Specifics["MD"]);
            }
        }
        WordClass partOfSpeechOrdinal;
        
        WordClass YearRange
        {
            get
            {
                return yearRange ?? (yearRange = WordClasses.Specifics["NNT2"]);
            }
        }
        WordClass yearRange;

        #endregion

        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            int last = s.Length - 1;
            if ( last < 2 )
            {
                yield return new WordInstance(s);
                yield break;
            }

            char one = Char.ToLower(s[last - 1]);
            char two = Char.ToLower(s[last]);

            if (yearTest.IsMatch(s))
                yield return new WordInstance(s, YearRange);

            else if( one == 's' && two == 't' )
                yield return new WordInstance(s, PartOfSpeechOrdinal);

            else if( one == 'n' && two == 'd' )
                yield return new WordInstance(s, PartOfSpeechOrdinal);

            else if (one == 'r' && two == 'd')
                yield return new WordInstance(s, PartOfSpeechOrdinal);

            else if (one == 't' && two == 'h')
                yield return new WordInstance(s, PartOfSpeechOrdinal);

            else
                yield return new WordInstance(s);
        }

        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            return this;
        }

        public override IParsingState OnPunctuation(IContext c)
        {
            if (c.Character == '.')
                return EndOfSentence;

            return base.OnPunctuation(c);
        }
    }
}
