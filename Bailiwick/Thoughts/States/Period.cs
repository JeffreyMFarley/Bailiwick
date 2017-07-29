using System;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Thoughts.States
{
    internal class Period : IState
    {
        #region Fields and Properties
        TransitionPredicate[] predicates = { 
                                               // The below functions are exceptions to the normal case
                                               CourtCase,
                                               // This is the normal case
                                               LowerDotUpper, 
                                               // The below functions are unhandled in the normal case and should be 'EndOfSentence'
                                               // They appear in decreasing order of frequency
                                               DotDot,
                                               DotExclamation,
                                               DotInterrogative,
                                               DotNewLine,
                                               Token_I_Dot,
                                               // The below functions are unhandled in the normal case and should be 'Accumulating'
                                               // They appear in decreasing order of frequency
                                               LetterDot, 
                                               TokenDotNumber,
                                               DotPunctuation,
                                               NumberTokenDot,
                                               LowerDotLower,
                                               SmallUpperDotUpper,
                                               UpperDotLower,
                                               NumberDotNumber,
                                               ZeroDegrees,
                                               // Nothing was handled, the default is to assume end of sentence
                                               Default
                                           };

        static string[] tokensOkToLeadNumbers = { 
                                                    "ca", "ch", "chap", 
                                                    "eq", "eqn", "eqns",
                                                    "fig", "figs", 
                                                    "no", "nos",
                                                    "op",
                                                    "pg", "pt", "pp",
                                                    "rul",
                                                    "sec", "spec", "stat"
                                                };

        static string[] tokensOkToFollowNumbers = { 
                                                    "cc", "in", "sec",
                                                    "kc", "mc", "meq"     // Brown specific
                                                };
        
        static string[] tokensOkToLeadI = { "am", "do", "not", "than" };

        #endregion
        #region IState members

        public bool Ready
        {
            get { return false; }
        }

        public void Enter(IContext c, WordInstance wi)
        {
            c.Next.Enqueue(wi);
        }

        void AttachPeriodToPrevious(IContext c)
        {
            var prev = c.Pop();
            var appended = new WordInstance(prev.Instance + '.', prev);
            c.Push(appended);
        }

        public void OnWord(IContext c, WordInstance wi)
        {
            var thePeriod = c.Next.Dequeue();

            var newState = StateFactory.HasTransition(predicates, c, wi);
            if (newState == StateFactory.Accumulate || wi.PartOfSpeech == WordClasses.NewLine)
            {
                AttachPeriodToPrevious(c);
                c.Push(wi);
            }
            else if (wi.Instance == "." )
            {
                AttachPeriodToPrevious(c);
                c.Push(wi);
            }
            else if (newState == StateFactory.EndOfSentence)
            {
                c.Push(thePeriod);
                c.Next.Enqueue(wi);
            }
            else if (newState == StateFactory.Exclamation || newState == StateFactory.Interrogative)
            {
                AttachPeriodToPrevious(c);
            }

            if (newState != null)
                c.Transfer(newState);
        }

        public override int GetHashCode()
        {
            return 2;
        }

        #endregion
        #region Predicates

        // Normal case
        static IState CourtCase(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if (pw.Instance == "v" && Char.IsUpper(w.Instance[0]))
                return StateFactory.Accumulate;

            return null;
        }

        static IState DotDot(IContext c, WordInstance w)
        {
            if( w.Instance == "." )
                return StateFactory.EndOfSentence;

            var pw = c.Peek;
            if( pw.Instance.EndsWith(".") )
                return StateFactory.EndOfSentence;

            return null;
        }

        static IState DotExclamation(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.Exclamation) ? StateFactory.Exclamation : null;
        }

        static IState DotInterrogative(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.Question) ? StateFactory.Interrogative : null;
        }

        static IState DotNewLine(IContext c, WordInstance w)
        {
            return (w.PartOfSpeech == WordClasses.NewLine) ? StateFactory.EndOfSentence : null;
        }

        static IState DotPunctuation(IContext c, WordInstance w)
        {
            if (w.IsQuote() || w.PartOfSpeech == WordClasses.Hyphen)
                return StateFactory.EndOfSentence;

            return (w.GeneralWordClass == WordClassType.Punctuation) ? StateFactory.Accumulate : null;
        }

        static IState LetterDot(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            return (pw.Instance.Length == 1 && char.IsLetter(pw.Instance[0])) ? StateFactory.Accumulate : null;
        }

        static IState LowerDotLower(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if (pw.Instance.Length >= 1 && Char.IsLower(pw.Instance[0]) && Char.IsLower(w.Instance[0]))
                return StateFactory.Accumulate;

            return null;
        }

        static IState LowerDotUpper(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if (pw.Instance.Length >= 1 && Char.IsLower(pw.Instance[0]) && Char.IsUpper(w.Instance[0]))
                return StateFactory.EndOfSentence;

            return null;
        }

        static IState NumberDotNumber(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            return (pw.PartOfSpeech == WordClasses.Number && w.PartOfSpeech == WordClasses.Number) ? StateFactory.Accumulate : null;
        }

        static IState NumberTokenDot(IContext c, WordInstance w)
        {
            var buffer = c.Buffer;
            var last = buffer.Count() - 1;
            if (last < 1)
                return null;

            if (tokensOkToFollowNumbers.Contains(buffer[last].Normalized) && buffer[last - 1].GeneralWordClass == WordClassType.Number)
                return StateFactory.Accumulate;

            return null;
        }

        static IState SmallUpperDotUpper(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if (pw.Instance.Length >= 1 && Char.IsUpper(pw.Instance[0]) && Char.IsUpper(w.Instance[0]) && pw.Instance.Length <= 4)
            //                return StateFactory.Accumulate;
            {
                //Console.Write(c.Peek);
                //Console.Write("\t.\t");
                //Console.WriteLine(w);
            }

            return null;
        }

        static IState TokenDotNumber(IContext c, WordInstance w)
        {
            var pw = c.Peek.Normalized;
            if (!tokensOkToLeadNumbers.Contains(pw))
                return null;

            if (w.GeneralWordClass == WordClassType.Number)
                return StateFactory.Accumulate;

            // Catch-all 
            if (Char.IsDigit(w.Instance[0]))
                return StateFactory.Accumulate;

            return null;
        }

        static IState Token_I_Dot(IContext c, WordInstance w)
        {
            var buffer = c.Buffer;
            var last = buffer.Count() - 1;
            if (last < 1)
                return null;

            if (buffer[last].Instance == "I" && tokensOkToLeadI.Contains(buffer[last - 1].Normalized))
                return StateFactory.EndOfSentence;

            return null;
        }

        static IState UpperDotLower(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if (pw.Instance.Length >= 1 && Char.IsUpper(pw.Instance[0]) && Char.IsLower(w.Instance[0]))
                return StateFactory.Accumulate;

            return null;
        }

        static IState ZeroDegrees(IContext c, WordInstance w)
        {
            var pw = c.Peek;
            if( pw.Instance == "0C" || pw.Instance == "0F" || w.Instance == "0F" )
                return StateFactory.Accumulate;

            return null;
        }

        // Last Resort
        static IState Default(IContext c, WordInstance w)
        {
            //Console.Write(c.Peek);
            //Console.Write("\t.\t");
            //Console.WriteLine(w);
            return StateFactory.EndOfSentence;
        }

        #endregion
    }
}
