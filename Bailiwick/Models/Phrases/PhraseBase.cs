using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    abstract public class PhraseBase : IPhrase
    {
        #region PhraseBase Members
        
        public List<WordInstance> Adjuncts
        {
            get { return adjuncts ?? (adjuncts = new List<WordInstance>()); }
        }
        List<WordInstance> adjuncts;

        virtual protected void ResetIndexes()
        {
            // Reset the indexes
            startIndex = null;
            endIndex = null;
        }

        protected void SetIndexes(WordInstance word)
        {
            if (startIndex == null)
                StartIndex = word.StartIndex;

            EndIndex = word.EndIndex;
        }

        #endregion

        #region IEnumerable Members

        abstract public IEnumerator<WordInstance> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
        
        #region IPhrase Members

        abstract public WordClassType GeneralWordClass
        {
            get;
        }

        abstract public WordInstance Head
        {
            get;
        }

        public IList<WordInstance> ClassColocates
        {
            get
            {
                if (classColocates == null)
                {
                    classColocates = new List<WordInstance>();
                }
                return classColocates;
            }
        }
        List<WordInstance> classColocates;

        public IList<WordInstance> AdjunctColocates
        {
            get { return adjunctColocates ?? (adjunctColocates = new List<WordInstance>()); }
        }
        List<WordInstance> adjunctColocates;

        public IList<WordInstance> Negatives
        {
            get { return negatives ?? (negatives = new List<WordInstance>()); }
        }
        List<WordInstance> negatives;

        public int StartIndex
        {
            get
            {
                if (startIndex == null)
                {
                    startIndex = this.Min(x => x.StartIndex);
                }
                return startIndex.Value;
            }
            protected set
            {
                endIndex = value;
            }
        }
        int? startIndex;

        public int EndIndex
        {
            get
            {
                if (endIndex == null)
                {
                    endIndex = this.Max(x => x.EndIndex);
                }
                return endIndex.Value;
            }
            protected set
            {
                endIndex = value;
            }
        }
        int? endIndex;

        abstract public bool IsSupported(WordInstance word);

        abstract public void AddHead(WordInstance word);
        
        abstract public void AddTail(WordInstance word);

        abstract public bool Remove(WordInstance word);
        
        #endregion

        public override string ToString()
        {
            var wi = Head;
            return (wi == null) ? "[N/A]" : Head.ToString();
        }

    }
}
