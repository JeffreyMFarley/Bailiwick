using System.Collections.Generic;

namespace Bailiwick.Models
{
    public class GeneralWordClassComparer : IEqualityComparer<WordClass>
    {
        #region IEqualityComparer<WordClass> Members

        public bool Equals(WordClass x, WordClass y)
        {
            if (x == null && y != null)
                return false;

            if (x != null && y == null)
                return false;

            if (x == null && y == null)
                return true;

            return x.General == y.General;
        }

        public int GetHashCode(WordClass obj)
        {
            return (int)obj.General;
        }

        #endregion
    }
}

