using System;
using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models
{
    public class Colocates : IEquatable<Colocates>, IComparable<Colocates>
    {
        private IList<WordInstance> locates;

        public Colocates(IList<WordInstance> words, WordClassType wordClassType, WordInstance head)
        {
            locates = new List<WordInstance>(words);
            WordClassType = wordClassType;
            Head = (head != null) ? head : locates.Last();
        }

        public WordClassType WordClassType { get; private set; }

        public WordInstance Head { get; set; }

        public int Length
        {
            get { return locates.Count; }
        }

        int hash = 0;
        public override int GetHashCode()
        {
            if (hash == 0)
            {
                for (int i = 0; i < locates.Count; i++)
                {
                    hash ^= GetString(locates[i]).GetHashCode();
                }
            }

            return hash;
        }

        string representation;
        public override string ToString()
        {
            if( string.IsNullOrEmpty(representation) )
                representation = string.Join(" ", locates.Select(l => l.Instance));

            return representation;
        }

        #region IEquatable<Colocates> Members

        public bool Equals(Colocates other)
        {
            if (other == null)
                return false;

            if( locates.Count != other.locates.Count )
                return false;

            for (int i = 0; i < locates.Count; i++)
            {
                if (GetString(locates[i]) != GetString(other.locates[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Colocates);
        }

        #endregion

        #region IComparable<Colocates> Members

        public int CompareTo(Colocates other)
        {
            if (other == null)
                return 1;

            if( other.Length > Length )
                return -1;

            if( other.Length < Length )
                return 1;

            return ToString().CompareTo(other.ToString());
        }
        
        #endregion

        string GetString(WordInstance wi)
        {
            return string.IsNullOrEmpty(wi.Lemma) ? wi.Normalized : wi.Lemma;
        }

    }
}
