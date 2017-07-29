using System;
using System.Runtime.Serialization;

namespace Bailiwick.Models
{
    [DataContract]
    public class SyntaxPair : IComparable<SyntaxPair>, IComparable, IEquatable<SyntaxPair>
    {
        public SyntaxPair(WordClassType previous, WordClassType next)
        {
            Previous = previous;
            Next = next;
        }

        [DataMember]
        public WordClassType Previous
        {
            get;
            private set;
        }

        [DataMember]
        public WordClassType Next
        {
            get;
            private set;
        }

        public override int GetHashCode()
        {
 	         return Previous.GetHashCode() ^ Next.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:G} - {1:G}", Previous, Next);
        }

        #region IComparable Members

        public int CompareTo(SyntaxPair other)
        {
            if (other == null )
                return -1;

            int i = Previous.CompareTo(other.Previous);

            if (i == 0)
                i = Next.CompareTo(other.Next);

            return i;
        }

        public int CompareTo(object other)
        {
            return CompareTo(other as SyntaxPair);
        }

        #endregion

        #region IEquatable<SyntaxPair> Members

        public bool Equals(SyntaxPair other)
        {
            if (other == null )
                return false;

            if (Previous != other.Previous)
                return false;

            return Next.Equals(other.Next);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SyntaxPair);
        }

        #endregion
    }
}