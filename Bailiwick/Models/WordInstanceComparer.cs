using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models
{
    public class WordInstanceComparer : IComparer<WordInstance>, IEqualityComparer<WordInstance>
    {
        #region IComparer<WordInstance> Members

        public int Compare(WordInstance x, WordInstance y)
        {
            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x == null && y == null)
                return 0;

            int i = x.Instance.CompareTo(y.Instance);
            if (i == 0)
                i = x.Normalized.CompareTo(y.Normalized);
            if (i == 0)
                i = x.PartOfSpeech.Specific.CompareTo(y.PartOfSpeech.Specific);

            return i;
        }

        #endregion

        #region IEqualityComparer<WordInstance> Members

        public bool Equals(WordInstance x, WordInstance y)
        {
            if (x == null && y != null)
                return false;

            if (x != null && y == null)
                return false;

            if (x == null && y == null)
                return true;

            if (x.Instance != y.Instance )
                return false;

            if (x.Normalized != y.Normalized)
                return false;

            return x.PartOfSpeech.Specific == y.PartOfSpeech.Specific;
        }

        public int GetHashCode(WordInstance obj)
        {
            if( obj == null )
                return 0;

            return obj.Instance.GetHashCode() ^ obj.PartOfSpeech.GetHashCode();
        }

        #endregion
    }
}
