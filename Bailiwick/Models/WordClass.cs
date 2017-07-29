using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Bailiwick.Models
{
    /// <summary>
    /// aka WordClass
    /// </summary>
    [DataContract]
    public enum WordClassType : int
    {
        [EnumMember]
        Start = 0,
        [EnumMember]
        Article = 'a',
        [EnumMember]
        BeforeClauseMarker = 'b',
        [EnumMember]
        Conjunction = 'c',
        [EnumMember]
        Determiner = 'd',
        [EnumMember]
        Existential = 'e',
        [EnumMember]
        Unclassified = 'f',
        [EnumMember]
        GenitiveMarker = 'g',
        [EnumMember]
        Adposition = 'i',
        [EnumMember]
        Adjective = 'j',
        [EnumMember]
        Number = 'm',
        [EnumMember]
        Noun = 'n',
        [EnumMember]
        Pronoun = 'p',
        [EnumMember]
        Punctuation = 'q',
        [EnumMember]
        Adverb = 'r',
        [EnumMember]
        InfinitiveMarker = 't',
        [EnumMember]
        Interjection = 'u',
        [EnumMember]
        Verb = 'v',
        [EnumMember]
        Not = 'x',
        [EnumMember]
        Letter = 'z'
    }

    public class WordClass
    {
        static public GeneralWordClassComparer GeneralComparer = new GeneralWordClassComparer();

        public readonly WordClassType General;
        public readonly string Specific;
        public readonly string Description;

        public WordClass(WordClassType general, string specific, string description)
        {
            General = general;
            Specific = specific;
            Description = description;
        }

        public override int GetHashCode()
        {
            return Specific.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:G} - {1}", General, Description);
        }
    }
    /// <summary>
    /// Holds the CLAWS7 tagging for classifying parts of speech
    /// </summary>
    static public class WordClasses
    {
        #region Standard Instances

        static public readonly WordClass Article = new WordClass(WordClassType.Article, "AT", "Article");
        static public readonly WordClass BeforeClauseMarker = new WordClass(WordClassType.BeforeClauseMarker, "B", "Before Clause Marker");
        static public readonly WordClass CoordinatingConjunction = new WordClass(WordClassType.Conjunction, "CC", "Coordinating Conjunction");
        static public readonly WordClass SubordinatingConjunction = new WordClass(WordClassType.Conjunction, "CS", "Subordinating Conjunction");
        static public readonly WordClass Determiner = new WordClass(WordClassType.Determiner, "DD", "Determiner");
        static public readonly WordClass Existential = new WordClass(WordClassType.Existential, "EX", "Existential there");
        static public readonly WordClass Unclassified = new WordClass(WordClassType.Unclassified, "F", "Unclassified");
        static public readonly WordClass GenitiveMarker = new WordClass(WordClassType.GenitiveMarker, "GE", "Genitive Marker");
        static public readonly WordClass Adposition = new WordClass(WordClassType.Adposition, "II", "Preposition");
        static public readonly WordClass Adjective = new WordClass(WordClassType.Adjective, "JJ", "Adjective");
        static public readonly WordClass Number = new WordClass(WordClassType.Number, "MC", "cardinal, neutral for number");
        static public readonly WordClass Noun = new WordClass(WordClassType.Noun, "N", "Noun");
        static public readonly WordClass Pronoun = new WordClass(WordClassType.Pronoun, "P", "Pronoun");
        static public readonly WordClass Punctuation = new WordClass(WordClassType.Punctuation, "QQ", "General Punctuation");
        static public readonly WordClass Adverb = new WordClass(WordClassType.Adverb, "RR", "Adverb");
        static public readonly WordClass InfinitiveMarker = new WordClass(WordClassType.InfinitiveMarker, "TO", "Infinitive Marker");
        static public readonly WordClass Interjection = new WordClass(WordClassType.Interjection, "UH", "Interjection");
        static public readonly WordClass Verb = new WordClass(WordClassType.Verb, "V", "Verb");
        static public readonly WordClass Not = new WordClass(WordClassType.Not, "XX", "Not, n't");
        static public readonly WordClass Letter = new WordClass(WordClassType.Letter, "Z", "Letter");

        static public readonly WordClass EllipsisNoun = new WordClass(WordClassType.Noun, "NE", "Implied noun");

        static public readonly WordClass Comma = new WordClass(WordClassType.Punctuation, "QC", "Comma");
        static public readonly WordClass SingleQuote = new WordClass(WordClassType.Punctuation, "QSQ", "Single Quote");
        static public readonly WordClass DoubleQuote = new WordClass(WordClassType.Punctuation, "QD", "Double Quote");
        static public readonly WordClass Exclamation = new WordClass(WordClassType.Punctuation, "QEE", "Exclamation");
        static public readonly WordClass NewLine = new WordClass(WordClassType.Punctuation, "QEN", "New Line");
        static public readonly WordClass Period = new WordClass(WordClassType.Punctuation, "QEP", "Period");
        static public readonly WordClass Question = new WordClass(WordClassType.Punctuation, "QEQ", "Question");
        static public readonly WordClass Hyphen = new WordClass(WordClassType.Punctuation, "QH", "Hyphen");
        static public readonly WordClass Semicolon = new WordClass(WordClassType.Punctuation, "QS", "Semicolon");

        #endregion

        #region Flyweight Factory

        static public IDictionary<string, WordClass> Specifics
        {
            get
            {
                return specifics.Value;
            }
        }
        static Lazy<IDictionary<string, WordClass>> specifics = new Lazy<IDictionary<string, WordClass>>(BuildFlyweights, true);

        static IDictionary<string, WordClass> BuildFlyweights()
        {
            var general = new WordClass[] {  Article,
                                             BeforeClauseMarker,
                                             CoordinatingConjunction,
                                             SubordinatingConjunction,
                                             Determiner,
                                             Existential, 
                                             Unclassified,
                                             GenitiveMarker,
                                             Adposition,
                                             Adjective,
                                             Number,
                                             Noun,
                                             Pronoun,
                                             Punctuation,
                                             Adverb,
                                             InfinitiveMarker,
                                             Interjection,
                                             Verb,
                                             Not,
                                             Letter,
                                             Comma,
                                             SingleQuote,
                                             DoubleQuote,
                                             Exclamation,
                                             NewLine,
                                             Period,
                                             Question,
                                             Hyphen,
                                             Semicolon
            };

            var result = general.ToDictionary(k => k.Specific);

            result.Add("APPGE", new WordClass((WordClassType)'a', "APPGE", "possessive pronoun, pre-nominal"));
            result.Add("AT1", new WordClass((WordClassType)'a', "AT1", "singular article"));
            result.Add("BCL", new WordClass((WordClassType)'b', "BCL", "before-clause marker"));
            result.Add("CCB", new WordClass((WordClassType)'c', "CCB", "adversative coordinating"));
            result.Add("CSA", new WordClass((WordClassType)'c', "CSA", "as"));
            result.Add("CSN", new WordClass((WordClassType)'c', "CSN", "than"));
            result.Add("CST", new WordClass((WordClassType)'c', "CST", "that"));
            result.Add("CSW", new WordClass((WordClassType)'c', "CSW", "whether"));
            result.Add("DA", new WordClass((WordClassType)'d', "DA", "after-determiner or post-determiner capable of pronominal function"));
            result.Add("DA1", new WordClass((WordClassType)'d', "DA1", "singular after-determiner"));
            result.Add("DA2", new WordClass((WordClassType)'d', "DA2", "plural after-determiner"));
            result.Add("DAR", new WordClass((WordClassType)'d', "DAR", "comparative after-determiner"));
            result.Add("DAT", new WordClass((WordClassType)'d', "DAT", "superlative after-determiner"));
            result.Add("DB", new WordClass((WordClassType)'d', "DB", "before determiner or pre-determiner capable of pronominal function"));
            result.Add("DB2", new WordClass((WordClassType)'d', "DB2", "plural before-determiner"));
            result.Add("DD1", new WordClass((WordClassType)'d', "DD1", "singular"));
            result.Add("DD2", new WordClass((WordClassType)'d', "DD2", "plural"));
            result.Add("DDQ", new WordClass((WordClassType)'d', "DDQ", "wh-"));
            result.Add("DDQGE", new WordClass((WordClassType)'d', "DDQGE", "wh-, genitive"));
            result.Add("DDQV", new WordClass((WordClassType)'d', "DDQV", "wh-ever"));
            result.Add("FO", new WordClass((WordClassType)'f', "FO", "formula"));
            result.Add("FU", new WordClass((WordClassType)'f', "FU", "unclassified word"));
            result.Add("FW", new WordClass((WordClassType)'f', "FW", "foreign word"));
            result.Add("IF", new WordClass((WordClassType)'i', "IF", "for"));
            result.Add("IO", new WordClass((WordClassType)'i', "IO", "of"));
            result.Add("IW", new WordClass((WordClassType)'i', "IW", "with, without"));
            result.Add("JJR", new WordClass((WordClassType)'j', "JJR", "general comparative"));
            result.Add("JJT", new WordClass((WordClassType)'j', "JJT", "general superlative"));
            result.Add("JK", new WordClass((WordClassType)'j', "JK", "catenative"));
            result.Add("MC1", new WordClass((WordClassType)'m', "MC1", "singular cardinal"));
            result.Add("MC2", new WordClass((WordClassType)'m', "MC2", "plural cardinal"));
            result.Add("MCGE", new WordClass((WordClassType)'m', "MCGE", "genitive cardinal, neutral for number"));
            result.Add("MCMC", new WordClass((WordClassType)'m', "MCMC", "hyphenated number"));
            result.Add("MD", new WordClass((WordClassType)'m', "MD", "ordinal"));
            result.Add("MF", new WordClass((WordClassType)'m', "MF", "fraction, neutral for number"));
            result.Add("ND1", new WordClass((WordClassType)'n', "ND1", "singular noun of direction"));
            result.Add("NN", new WordClass((WordClassType)'n', "NN", "common noun, neutral for number"));
            result.Add("NN1", new WordClass((WordClassType)'n', "NN1", "singular common noun"));
            result.Add("NN2", new WordClass((WordClassType)'n', "NN2", "plural common noun"));
            result.Add("NNA", new WordClass((WordClassType)'n', "NNA", "following noun of title"));
            result.Add("NNB", new WordClass((WordClassType)'n', "NNB", "preceding noun of title"));
            result.Add("NNL1", new WordClass((WordClassType)'n', "NNL1", "singular locative noun"));
            result.Add("NNL2", new WordClass((WordClassType)'n', "NNL2", "plural locative noun"));
            result.Add("NNO", new WordClass((WordClassType)'n', "NNO", "numeral noun, neutral for number"));
            result.Add("NNO2", new WordClass((WordClassType)'n', "NNO2", "numeral noun, plural"));
            result.Add("NNT1", new WordClass((WordClassType)'n', "NNT1", "temporal noun, singular"));
            result.Add("NNT2", new WordClass((WordClassType)'n', "NNT2", "temporal noun, plural"));
            result.Add("NNU", new WordClass((WordClassType)'n', "NNU", "unit of measurement, neutral for number"));
            result.Add("NNU1", new WordClass((WordClassType)'n', "NNU1", "singular unit of measurement"));
            result.Add("NNU2", new WordClass((WordClassType)'n', "NNU2", "plural unit of measurement"));
            result.Add("NP", new WordClass((WordClassType)'n', "NP", "proper noun, neutral for number"));
            result.Add("NP1", new WordClass((WordClassType)'n', "NP1", "singular proper"));
            result.Add("NP2", new WordClass((WordClassType)'n', "NP2", "plural proper"));
            result.Add("NPD1", new WordClass((WordClassType)'n', "NPD1", "singular weekday"));
            result.Add("NPD2", new WordClass((WordClassType)'n', "NPD2", "plural weekday"));
            result.Add("NPM1", new WordClass((WordClassType)'n', "NPM1", "singular month"));
            result.Add("NPM2", new WordClass((WordClassType)'n', "NPM2", "plural month"));
            result.Add("PN", new WordClass((WordClassType)'p', "PN", "indefinite pronoun, neutral for number"));
            result.Add("PN1", new WordClass((WordClassType)'p', "PN1", "indefinite pronoun, singular"));
            result.Add("PN2", new WordClass((WordClassType)'p', "PN2", "indefinite pronoun, plural"));
            result.Add("PNQO", new WordClass((WordClassType)'p', "PNQO", "objective wh-"));
            result.Add("PNQS", new WordClass((WordClassType)'p', "PNQS", "subjective wh-"));
            result.Add("PNQV", new WordClass((WordClassType)'p', "PNQV", "wh-ever"));
            result.Add("PNX1", new WordClass((WordClassType)'p', "PNX1", "reflexive indefinite"));
            result.Add("PPGE", new WordClass((WordClassType)'p', "PPGE", "nominal possessive personal"));
            result.Add("PPH1", new WordClass((WordClassType)'p', "PPH1", "3rd person sing. neuter personal"));
            result.Add("PPHO1", new WordClass((WordClassType)'p', "PPHO1", "3rd person sing. objective personal"));
            result.Add("PPHO2", new WordClass((WordClassType)'p', "PPHO2", "3rd person plural objective personal"));
            result.Add("PPHS1", new WordClass((WordClassType)'p', "PPHS1", "3rd person sing. subjective personal"));
            result.Add("PPHS2", new WordClass((WordClassType)'p', "PPHS2", "3rd person plural subjective personal"));
            result.Add("PPIO1", new WordClass((WordClassType)'p', "PPIO1", "1st person sing. objective personal"));
            result.Add("PPIO2", new WordClass((WordClassType)'p', "PPIO2", "1st person plural objective personal"));
            result.Add("PPIS1", new WordClass((WordClassType)'p', "PPIS1", "1st person sing. subjective personal"));
            result.Add("PPIS2", new WordClass((WordClassType)'p', "PPIS2", "1st person plural subjective personal"));
            result.Add("PPX1", new WordClass((WordClassType)'p', "PPX1", "singular reflexive personal"));
            result.Add("PPX2", new WordClass((WordClassType)'p', "PPX2", "plural reflexive personal"));
            result.Add("PPY", new WordClass((WordClassType)'p', "PPY", "2nd person personal"));

            result.Add("QCA", new WordClass((WordClassType)'q', "QCA", "adjective comma"));
            result.Add("QCL", new WordClass((WordClassType)'q', "QCL", "list comma"));
            result.Add("QCP", new WordClass((WordClassType)'q', "QCP", "parenthetical comma"));
            result.Add("QCQ", new WordClass((WordClassType)'q', "QCQ", "quoted material comma"));
            result.Add("QCR", new WordClass((WordClassType)'q', "QCR", "adverbial comma"));
            result.Add("QCS", new WordClass((WordClassType)'q', "QCS", "separation of clause comma"));

            result.Add("RA", new WordClass((WordClassType)'r', "RA", "adverb, after nominal head"));
            result.Add("REX", new WordClass((WordClassType)'r', "REX", "introducing appositional constructions"));
            result.Add("RG", new WordClass((WordClassType)'r', "RG", "degree"));
            result.Add("RGA", new WordClass((WordClassType)'r', "RGA", "comparative 'as'"));
            result.Add("RGQ", new WordClass((WordClassType)'r', "RGQ", "wh- degree"));
            result.Add("RGQV", new WordClass((WordClassType)'r', "RGQV", "wh-ever degree"));
            result.Add("RGR", new WordClass((WordClassType)'r', "RGR", "comparative degree"));
            result.Add("RGT", new WordClass((WordClassType)'r', "RGT", "superlative degree"));
            result.Add("RL", new WordClass((WordClassType)'r', "RL", "locative"));
            result.Add("RLR", new WordClass((WordClassType)'r', "RLR", "locative, comparative"));
            result.Add("RLT", new WordClass((WordClassType)'r', "RLT", "locative, superlative"));
            result.Add("RP", new WordClass((WordClassType)'r', "RP", "prep. adverb, particle"));
            result.Add("RPK", new WordClass((WordClassType)'r', "RPK", "prep. adv., catenative"));
            result.Add("RRQ", new WordClass((WordClassType)'r', "RRQ", "wh- general"));
            result.Add("RRQV", new WordClass((WordClassType)'r', "RRQV", "wh-ever general"));
            result.Add("RRR", new WordClass((WordClassType)'r', "RRR", "comparative general"));
            result.Add("RRT", new WordClass((WordClassType)'r', "RRT", "superlative general"));
            result.Add("RT", new WordClass((WordClassType)'r', "RT", "quasi-nominal adverb of time"));
            result.Add("RTR", new WordClass((WordClassType)'r', "RTR", "quasi-nominal adverb of time, comparative"));
            result.Add("RTT", new WordClass((WordClassType)'r', "RTT", "quasi-nominal adverb of time, superlative"));
            result.Add("VB0", new WordClass((WordClassType)'v', "VB0", "be, base form"));
            result.Add("VBDR", new WordClass((WordClassType)'v', "VBDR", "were"));
            result.Add("VBDZ", new WordClass((WordClassType)'v', "VBDZ", "was"));
            result.Add("VBG", new WordClass((WordClassType)'v', "VBG", "being"));
            result.Add("VBI", new WordClass((WordClassType)'v', "VBI", "be, infinitive"));
            result.Add("VBM", new WordClass((WordClassType)'v', "VBM", "am"));
            result.Add("VBN", new WordClass((WordClassType)'v', "VBN", "been"));
            result.Add("VBR", new WordClass((WordClassType)'v', "VBR", "are"));
            result.Add("VBZ", new WordClass((WordClassType)'v', "VBZ", "is"));
            result.Add("VD0", new WordClass((WordClassType)'v', "VD0", "do, base form"));
            result.Add("VDD", new WordClass((WordClassType)'v', "VDD", "did"));
            result.Add("VDG", new WordClass((WordClassType)'v', "VDG", "doing"));
            result.Add("VDI", new WordClass((WordClassType)'v', "VDI", "do, infinitive"));
            result.Add("VDN", new WordClass((WordClassType)'v', "VDN", "done"));
            result.Add("VDZ", new WordClass((WordClassType)'v', "VDZ", "does"));
            result.Add("VH0", new WordClass((WordClassType)'v', "VH0", "have, base form"));
            result.Add("VHD", new WordClass((WordClassType)'v', "VHD", "had"));
            result.Add("VHG", new WordClass((WordClassType)'v', "VHG", "having"));
            result.Add("VHI", new WordClass((WordClassType)'v', "VHI", "have"));
            result.Add("VHN", new WordClass((WordClassType)'v', "VHN", "had"));
            result.Add("VHZ", new WordClass((WordClassType)'v', "VHZ", "has"));
            result.Add("VM", new WordClass((WordClassType)'v', "VM", "modal auxiliary"));
            result.Add("VMK", new WordClass((WordClassType)'v', "VMK", "modal catenative"));
            result.Add("VV0", new WordClass((WordClassType)'v', "VV0", "base form of lexical"));
            result.Add("VVD", new WordClass((WordClassType)'v', "VVD", "past tense of lexical"));
            result.Add("VVG", new WordClass((WordClassType)'v', "VVG", "-ing participle of lexical"));
            result.Add("VVGK", new WordClass((WordClassType)'v', "VVGK", "-ing participle catenative"));
            result.Add("VVI", new WordClass((WordClassType)'v', "VVI", "infinitive"));
            result.Add("VVN", new WordClass((WordClassType)'v', "VVN", "past participle of lexical"));
            result.Add("VVNK", new WordClass((WordClassType)'v', "VVNK", "past participle catenative"));
            result.Add("VVZ", new WordClass((WordClassType)'v', "VVZ", "-s form of lexical verb"));
            result.Add("ZZ1", new WordClass((WordClassType)'z', "ZZ1", "singular letter"));
            result.Add("ZZ2", new WordClass((WordClassType)'z', "ZZ2", "plural letter"));

            return result;
        }
        #endregion

    }
}
