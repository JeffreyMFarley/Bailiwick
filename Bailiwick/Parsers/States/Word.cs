using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class Word : ParsingStateBase
    {
        public WordClass Not
        {
            get
            {
                return not ?? (not = WordClasses.Specifics["XX"]);
            }
        }
        WordClass not;

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
            if (apostrophes.Contains(c.Character))
                return xApostrophe;

            switch (c.Character)
            {
                case '.':
                    return xPeriod;
            }

            return base.OnPunctuation(c);
        }

        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            // Only check for one contraction
            if (s.Length == 6
             && Char.ToLower(s[0]) == 'c'
             && Char.ToLower(s[1]) == 'a'
             && Char.ToLower(s[2]) == 'n'
             && Char.ToLower(s[3]) == 'n'
             && Char.ToLower(s[4]) == 'o'
             && Char.ToLower(s[5]) == 't'
                )
            {
                yield return new WordInstance(new string(new char[] { s[0], s[1], s[2] }));
                yield return new WordInstance(new string(new char[] { s[3], s[4], s[5] }), Not);
                yield break;
            }

            yield return new WordInstance(s);

            /*
             * Add these
             * ahm
             * anythin	PN1
             * bein	VBG
             * buncha
             * coulda
             * didn
             * doan
             * gimme
             * gonna
             * gotta
             * hafta
             * howda
             * lemme
             * lookit
             * musta
             * nothin	PN1
             * oughta
             * outta
             * shouldda
             * wanna
             * wanta
             * whaddya
             * willya
             * woulda 
             */
        }
    }
}
