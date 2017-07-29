using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class Contraction : ParsingStateBase
    {
        public override IParsingState OnDone(IContext c)
        {
            return this;
        }

        public override IParsingState OnLetter(IContext c)
        {
            return this;
        }

        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            // Find the apostrophe
            int apos = s.IndexOfAny(apostrophes);
            Debug.Assert(apos != -1);
            int before = apos - 1;
            int after = apos + 1;
            int last = s.Length - 1;

            // Starting apostrophe - Open quote?  dialect cuteness?  don't care
            if (apos == 0)
            {
                yield return new WordInstance(s[0].ToString(), WordClasses.SingleQuote);
                yield return new WordInstance(s.Substring(1, last)); 
                yield break;
            }

            // Ending apostrophe - Close quote, plural possesive or dropped G
            if (apos == last)
            {
                if (Char.ToLower(s[before]) == 's')
                {
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance("'s", WordClasses.GenitiveMarker);
                    yield break;
                }
                else if (before > 0 && Char.ToLower(s[before - 1]) == 'i' && Char.ToLower(s[before]) == 'n')
                {
                    var w = s.Substring(0, apos);
                    yield return new WordInstance(w.Substitute(w + "g"));
                    yield break;
                }
                else
                {
                    foreach (var w in Word.Extract(c, s.Substring(0, apos)))
                        yield return w;

                    yield return new WordInstance(s[apos].ToString(), WordClasses.SingleQuote);
                    yield break;
                }
            }

            // Exactly one character after the apostrophe
            if (after == last)
            {
                if (Char.ToLower(s[after]) == 's' || Char.ToLower(s[after]) == 'd')
                {
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance(new string(new char[] { '\'', s[after] }));
                    yield break;
                }

                // Special cases
                if (string.Equals(s, "ain't", StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return new WordInstance(s.Substring(0, apos).Substitute("be"));
                    yield return new WordInstance(s.Substring(after, 1).Substitute("not"), WordClasses.Not);
                    yield break;
                }

                if (string.Equals(s, "won't", StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return new WordInstance(s.Substring(0, apos).Substitute("will"));
                    yield return new WordInstance(s.Substring(after, 1).Substitute("not"), WordClasses.Not);
                    yield break;
                }

                if (string.Equals(s, "can't", StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return new WordInstance(s.Substring(0, apos).Substitute("can"));
                    yield return new WordInstance(s.Substring(after, 1).Substitute("not"), WordClasses.Not);
                    yield break;
                }

                if (Char.ToLower(s[before]) == 'n' && Char.ToLower(s[after]) == 't')
                {
                    yield return new WordInstance(s.Substring(0, before));
                    yield return new WordInstance(new string(new char[] { s[before], '\'', s[after] }), WordClasses.Not);
                    yield break;
                }
            }

            // See about common contractions
            var back = s.Substring(apos, last - apos + 1);
            switch (back.ToLower())
            {
                case "'m":
                case "\u2019m":
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance("am");
                    yield break;

                case "'re":
                case "\u2019re":
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance("are");
                    yield break;

                case "'ve":
                case "\u2019ve":
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance("have");
                    yield break;

                case "'ll":
                case "\u2019ll":
                    yield return new WordInstance(s.Substring(0, apos));
                    yield return new WordInstance("will");
                    yield break;
            }

            yield return new WordInstance(s);

            /* Some others
             * 'tis
             * c'mon
             * d'you
             * lighter'n
             * more'n
             * y'know
             * 'bout	II
             * 'cept	II
             * 'ceptin'	II
             * 'em	PPHO1
             * 'emselves	PPX2
             * 'fore	CS
             * 'im	PPHO1
             * 'n'	CC
             * 'nother	DD1
             * 'nough	DA
             * 'round	II
             * 'till	II
             * an'	CC  
             * c'n	VM
             * ever'	AT
             * ever'body	PN1
             * f'r	II
             * s'posin'	CS
             * th'	AT
             * their's	PPGE
             * tho'	CS
             * y'r	APPGE
             * 'round	RR
             * ol'	JJ
             */
        }

    }
}
