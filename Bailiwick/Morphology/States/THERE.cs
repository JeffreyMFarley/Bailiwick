using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.Morphology.States
{
    internal class THERE : IState
    {
        #region IState Members

        public bool AcceptAsNext(WordInstance w, double percentage, IContext c)
        {
            return (c.Word.GeneralWordClass == WordClassType.Existential) 
                ? asExistential(w,percentage, c) 
                : asAdverb(w, percentage, c);
        }

        #endregion

        string[] okVerbs = { 
                               "be", "have", "appear", "come", "cling", 
                               "correspond", "develop", "emerge", "ensue",
                               "exist", "feel", "go", "happen", "issue", 
                               "lay", "lie",
                               "occur", "persist", "remain", "seem",
                               "tend"
                           };
        
        string[] adverbParticiples = { "VVG", "VVN", "VVGK", "VVNK" };
        
        bool asExistential(WordInstance w, double percentage, IContext c)
        {
            if (w.GeneralWordClass == WordClassType.Adverb)
                return true;

            if ( w.PartOfSpeech.Specific == "VM" )
                return true;

            if (w.GeneralWordClass == WordClassType.Verb)
                return okVerbs.Contains(w.Lemma) && !adverbParticiples.Contains(w.PartOfSpeech.Specific);

            return false;
        }

        bool asAdverb(WordInstance w, double percentage, IContext c)
        {
            if (w.GeneralWordClass == WordClassType.GenitiveMarker)
                return false;

            // When used as an adverb, 'THERE' sometimes depends on the previous word
            var prev = c.Previous.Word;
            if (prev.GeneralWordClass == WordClassType.Adposition)
                return w.Lemma != "be";

            return true;
        }
    }
}
