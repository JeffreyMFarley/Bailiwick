using System.Collections.Generic;

namespace Bailiwick.Models
{
    public class GlossComparer : IComparer<IGloss>, IEqualityComparer<IGloss>
    {
        #region IComparer<IGloss> Members

        public int Compare(IGloss x, IGloss y)
        {
            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x == null && y == null)
                return 0;

            int i = x.Normalized.CompareTo(y.Normalized);
            if (i == 0)
                i = x.PartOfSpeech.Specific.CompareTo(y.PartOfSpeech.Specific);
            if (i == 0)
                i = x.PartOfSpeech.General.CompareTo(y.PartOfSpeech.General);

            return i;
        }

        #endregion

        #region IEqualityComparer<IGloss> Members

        public bool Equals(IGloss x, IGloss y)
        {
            if (x == null && y != null)
                return false;

            if (x != null && y == null)
                return false;

            if (x == null && y == null)
                return true;

            if (x.Normalized != y.Normalized)
                return false;

            if (x.PartOfSpeech.Specific != y.PartOfSpeech.Specific)
                return false;

            return x.PartOfSpeech.General == y.PartOfSpeech.General;
        }

        public int GetHashCode(IGloss obj)
        {
            if (obj == null)
                return 0;

            return obj.Normalized.GetHashCode() ^ obj.PartOfSpeech.GetHashCode();
        }

        #endregion
    }
}

