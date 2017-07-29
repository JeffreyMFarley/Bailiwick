using System.Collections.Generic;

namespace Bailiwick.Models
{
    public class SentenceNodeComparer : IComparer<ISentenceNode>, IEqualityComparer<ISentenceNode>
    {
        public WordInstanceComparer WordComparer
        {
            get
            {
                return wordComparer ?? (wordComparer = new WordInstanceComparer());
            }
        }
        WordInstanceComparer wordComparer;

        #region IComparer<ISentenceNode> Members

        public int Compare(ISentenceNode x, ISentenceNode y)
        {
            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x == null && y == null)
                return 0;

            int i = x.StartIndex.CompareTo(y.StartIndex);
            if (i == 0)
                i = y.EndIndex.CompareTo(x.EndIndex);  // Longer is first
            if (i == 0)
                i = x.GeneralWordClass.CompareTo(y.GeneralWordClass);
            if (i == 0)
                i = WordComparer.Compare(x.Head, y.Head);

            return i;
        }

        #endregion

        #region IEqualityComparer<ISentenceNode> Members

        public bool Equals(ISentenceNode x, ISentenceNode y)
        {
            if (x == null && y != null)
                return false;

            if (x != null && y == null)
                return false;

            if (x == null && y == null)
                return true;

            if (x.StartIndex != y.StartIndex)
                return false;

            if (x.EndIndex != y.EndIndex)
                return false;

            if (x.GeneralWordClass != y.GeneralWordClass)
                return false;

            return WordComparer.Equals(x.Head, y.Head);
        }

        public int GetHashCode(ISentenceNode obj)
        {
            if (obj == null)
                return 0;

            return obj.StartIndex.GetHashCode() ^ obj.EndIndex.GetHashCode()^ obj.GeneralWordClass.GetHashCode();
        }

        #endregion
    }
}
