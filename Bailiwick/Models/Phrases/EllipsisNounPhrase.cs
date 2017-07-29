using System.Collections.Generic;

namespace Bailiwick.Models.Phrases
{
    public class EllipsisNounPhrase : NounPhrase
    {
        public EllipsisNounPhrase(IList<WordInstance> leadingWords, IPhrase trailingWords)
        {
            foreach(var w in leadingWords)
                AddTail(w);

            if( trailingWords != null )
                foreach (var w in trailingWords)
                    AddTail(w);
        }

        public override WordInstance Head
        {
            get
            {
                return head;
            }
        }
        WordInstance head = new WordInstance("…", Gloss.EllipsisNoun);
    }
}
