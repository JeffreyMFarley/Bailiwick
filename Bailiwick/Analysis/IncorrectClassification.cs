using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Analysis
{
    public class IncorrectClassification
    {
        // Factory methods
        static public IncorrectClassification ExtractErrors(ClassifierNode tail)
        {
            var current = tail;
            while (current != null && current.Root != null )
            {
                if (current.Impossible && !current.Root.Impossible)
                    return new IncorrectClassification { Previous = current.Root, Next = current };

                current = current.Root;
            }

            return null;
        }

        #region Properties

        public ClassifierNode Previous { get; set; }
        public ClassifierNode Next { get; set; }

        public bool Repaired { get; set; }
        
        #endregion

        public void Repair(IConvert convert)
        {
            Repaired = true;

            // Fix the problem
            if (Previous.Instance.GeneralWordClass == WordClassType.Article)
                convert.ToNounPhrasePart(Next.Instance);

            else if (Previous.Instance.Normalized == "there" && Next.Instance.Lemma == "be")
                convert.ToExistential(Previous.Instance);

            else if (Next.Instance.GeneralWordClass == WordClassType.Verb && !Next.Instance.IsGerund())
            {
                if( Previous.Instance.Normalized == "to" )
                    convert.ToInfinitive(Previous.Instance);
                else if( Previous.Instance.GeneralWordClass == WordClassType.Adposition )
                    convert.ToNoun(Next.Instance);
            }

            else if (Next.Instance.GeneralWordClass == WordClassType.Pronoun && Previous.Instance.GeneralWordClass == WordClassType.Noun)
                convert.ToVerb(Previous.Instance);

            else
                Repaired = false;
        }
    }
}
