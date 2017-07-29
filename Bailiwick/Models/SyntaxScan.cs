using System.Collections.Generic;
using WordIndex = System.Collections.Generic.KeyValuePair<int, Bailiwick.Models.WordInstance>;

namespace Bailiwick.Models
{
    public class SyntaxScan
    {
        public SyntaxScan() { }

        public SyntaxScan(SyntaxScan source)
        {
            LeadWordClassType = source.LeadWordClassType;
            IsComplete = source.IsComplete;
            IsComplex = source.IsComplex;
            Length = source.Length;
            
            Commas = source.Commas;
            ClausePunctuation = source.ClausePunctuation;
            CoordinatingConjunctions = source.CoordinatingConjunctions;
            SubordinatingConjunctions = source.SubordinatingConjunctions;
        }

        public WordClassType LeadWordClassType { get; set; }

        public bool IsComplete { get; set; }
        public bool IsComplex { get; set; }
        public int Length { get; set; }

        public int Commas { get; set; }
        public int ClausePunctuation { get; set; }
        public int CoordinatingConjunctions { get; set; }
        public int SubordinatingConjunctions { get; set; }

        static public SyntaxScan Create(Sentence sentence)
        {
            var builder = new SyntaxScanBuilder();
            foreach (var w in sentence.Words)
                builder.Update(w);

            return builder.Result;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Commas, CoordinatingConjunctions, ClausePunctuation, SubordinatingConjunctions);
        }
    }
}
