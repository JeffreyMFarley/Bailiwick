using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models
{
    static public class Extensions
    {
        #region WordClassType Extensions

        static public bool IsClosedWordClass(this WordClassType g)
        {
            switch (g)
            {
                case WordClassType.Adposition:
                case WordClassType.Article:
                case WordClassType.BeforeClauseMarker:
                case WordClassType.Conjunction:
                case WordClassType.Determiner:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.InfinitiveMarker:
                case WordClassType.Letter:
                case WordClassType.Not:
                case WordClassType.Pronoun:
                case WordClassType.Punctuation:
                case WordClassType.Start:
                    return true;

                default:
                    return false;
            }
        }

        static public bool IsAdjectivePhrasePart(this WordClassType g)
        {
            switch (g)
            {
                case WordClassType.Adposition:
                case WordClassType.Adjective:
                case WordClassType.Adverb:
                case WordClassType.Article:
                case WordClassType.Determiner:
                case WordClassType.Not:
                case WordClassType.Number:
                    return true;

                default:
                    return false;
            }
        }

        static public bool IsAdjectival(this WordClassType g)
        {
            return g == WordClassType.Adjective || g == WordClassType.Number;
        }

        static public bool IsNounPhrasePart(this WordClassType g)
        {
            switch (g)
            {
                case WordClassType.Adjective:
                case WordClassType.Adposition:
                case WordClassType.Adverb:
                case WordClassType.Article:
                case WordClassType.Determiner:
                case WordClassType.Existential:
                case WordClassType.GenitiveMarker:
                case WordClassType.Letter:
                case WordClassType.Not:
                case WordClassType.Noun:
                case WordClassType.Number:
                case WordClassType.Pronoun:
                    return true;

                default:
                    return false;
            }
        }

        static public bool IsVerbPhrasePart(this WordClassType g)
        {
            switch (g)
            {
                case WordClassType.Adposition:
                case WordClassType.Adverb:
                case WordClassType.InfinitiveMarker:
                case WordClassType.Not:
                case WordClassType.Verb:
                    return true;

                default:
                    return false;
            }
        }

        #endregion

        #region WordClass Extensions

        static public bool IsClosedWordClass(this WordClass wc)
        {
            if (IsClosedWordClass(wc.General))
                return true;

            switch (wc.Specific)
            {
                case "JK":
                case "RG":
                case "RGA":
                case "RGQ":
                case "RGQV":
                case "RP":
                case "RPK":
                case "RRQ":
                case "RRQV":
                case "RT":
                case "VB0":
                case "VBDR":
                case "VBDZ":
                case "VBG":
                case "VBI":
                case "VBM":
                case "VBN":
                case "VBR":
                case "VBZ":
                case "VD0":
                case "VDD":
                case "VDG":
                case "VDI":
                case "VDN":
                case "VDZ":
                case "VH0":
                case "VHD":
                case "VHG":
                case "VHI":
                case "VHN":
                case "VHZ":
                case "VM":
                case "VMK":
                    return true;
            }

            return false;
        }

        static public bool IsTemporal(this WordClass wc)
        {
            switch (wc.Specific)
            {
                case "NNT1":
                case "NNT2":
                    return true;
            }

            return false;
        }

        #endregion

        #region IGloss Extensions

        static public bool IsAdjectivePhrasePart(this IGloss g)
        {
            return IsAdjectivePhrasePart(g.PartOfSpeech.General)
                || g.PartOfSpeech.Specific == "QCA"
                || g.PartOfSpeech == WordClasses.DoubleQuote;
        }

        static public bool IsAdjectival(this IGloss g)
        {
            return IsAdjectival(g.PartOfSpeech.General);
        }

        static public bool IsAuxiallaryVerb(this IGloss g)
        {
            if (g.PartOfSpeech.General != WordClassType.Verb)
                return false;

            switch (g.PartOfSpeech.Specific)
            {
                case "VB0":
                case "VBDR":
                case "VBDZ":
                case "VBG":
                case "VBI":
                case "VBM":
                case "VBN":
                case "VBR":
                case "VBZ":
                case "VH0":
                case "VHD":
                case "VHG":
                case "VHI":
                case "VHN":
                case "VHZ":
                case "VM":
                case "VMK":
                    return true;
            }

            return false;
        }

        static public bool IsClosedWordClass(this IGloss g)
        {
            return g.PartOfSpeech.IsClosedWordClass();
        }

        static public bool IsCoordinatingConjunction(this IGloss g)
        {
            return g.PartOfSpeech.Specific.StartsWith("CC");
        }

        static public bool IsComparative(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "DAR":
                case "RRR":
                case "JJR":
                case "RGR":
                case "RTR":
                case "RLR":
                    return true;
            }

            return false;
        }

        static public bool IsEndOfSentence(this IGloss g)
        {
            if (g.PartOfSpeech.General != WordClassType.Punctuation)
                return false;

            switch (g.PartOfSpeech.Specific)
            {
                case "QEE":
                case "QEP":
                case "QEN":
                case "QEQ":
                    return true;

                default:
                    return false;
            }
        }

        // Yes, technically this should be called "present particple or gerund" but this is shorter
        static public bool IsGerund(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "VBG":
                case "VDG":
                case "VHG":
                case "VVG":
                case "VVGK":
                    return true;
            }

            return false;
        }

        static public bool IsInfinite(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "VB0":
                case "VBG":
                case "VBN":
                case "VDG":
                case "VDN":
                case "VHG":
                case "VHN":
                case "VVG":
                case "VVI":
                case "VVN":
                case "VVGK":
                case "VVNK": 
                    return true;
            }

            return false;
        }

        static public bool IsNounPhrasePart(this IGloss g)
        {
            return IsNounPhrasePart(g.PartOfSpeech.General)
                || g.PartOfSpeech.Specific == "QCA"
                || g.PartOfSpeech == WordClasses.DoubleQuote;
        }

        static public bool IsObjectivePronoun(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "PNQO":
                case "PPHO1":
                case "PPHO2":
                case "PPIO1":
                case "PPIO2":
                    return true;
            }

            return false;
        }

        static public bool IsParticiple(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "VBG":
                case "VBN":
                case "VDG":
                case "VDN":
                case "VHG":
                case "VHN":
                case "VVG":
                case "VVN":
                case "VVGK":
                case "VVNK":
                    return true;
            }

            return false;
        }

        static public bool IsPostDeterminer(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "DA":
                case "DA1":
                case "DA2":
                case "DAR":
                case "DAT":
                    return true;
            }

            return false;
        }

        static public bool IsPreDeterminer(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "DB":
                case "DB2":
                    return true;
            }

            return false;
        }

        static public bool IsReflexivePronoun(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "PPX1":
                case "PPX2":
                    return true;
            }

            return false;
        }

        static public bool IsSubjectivePronoun(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "PNQS":
                case "PPHS1":
                case "PPHS2":
                case "PPIS1":
                case "PPIS2":
                    return true;
            }

            return false;
        }

        static public bool IsSubordinatingConjunction(this IGloss g)
        {
            return g.PartOfSpeech.Specific.StartsWith("CS");
        }

        static public bool IsSuperlative(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "DAT":
                case "RRT":
                case "JJT":
                case "RGT":
                case "RTT":
                case "RLT":
                    return true;
            }

            return false;
        }

        static public bool IsTemporal(this IGloss g)
        {
            return IsTemporal(g.PartOfSpeech);
        }

        static public bool IsQuote(this IGloss g)
        {
            switch (g.PartOfSpeech.Specific)
            {
                case "QSQ":
                case "QD":
                    return true;

                default:
                    return false;
            }
        }

        static public bool IsVerbPhrasePart(this IGloss g)
        {
            return IsVerbPhrasePart(g.PartOfSpeech.General);
        }

        #endregion

        #region IPhrase Extensions

        static public void AddHead(this IPhrase phrase, IList<WordInstance> words)
        {
            foreach (var o in words.Reverse<WordInstance>())
                phrase.AddHead(o);
        }

        #endregion

        #region Sentence Extensions

        static public WordInstance NextNotAdverb(this Sentence s, WordInstance w)
        {
            WordInstance next = null;
            for (int i = w.StartIndex + 1; i < s.Words.Count; i++)
            {
                next = s.Words[i];
                if( next.GeneralWordClass != WordClassType.Adverb )
                    return next;
            }

            return null;         
        }

        #endregion

        #region string extensions

        static public string Substitute(this string s, string t)
        {
            var wordCase = new WordCaseStatus();
            wordCase.Check(s);
            return wordCase.Apply(t);
        }

        #endregion
    }
}
