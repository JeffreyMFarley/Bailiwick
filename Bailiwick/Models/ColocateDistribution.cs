using Esoteric.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Bailiwick.Models
{
    [Serializable]
    public class ColocateDistribution : Dictionary<Colocates, long>, IDistribution<Colocates>, ISupportsStatistics
    {
        public ColocateDistribution() : base() { }
        protected ColocateDistribution(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual IEnumerable<Colocates> Bins
        {
            get { return this.Keys; }
        }

        public virtual Colocates Mode
        {
            get
            {
                Colocates ret = null;
                long mode = 0;

                foreach (KeyValuePair<Colocates, long> pair in this)
                {
                    if (pair.Value > mode)
                    {
                        ret = pair.Key;
                        mode = pair.Value;
                    }
                }

                return ret;
            }
        }

        #region IDistribution<Colocates> Members

        public virtual void Increment(Colocates key)
        {
            if (!base.ContainsKey(key))
                base[key] = 1;
            else
            {
                long count = base[key];
                base[key] = count + 1;
            }

            Sum++;
            SquareSum = base.Values.Sum(v => v * v);

            if (base[key] > Max)
                Max = base[key];
        }

        public virtual void Decrement(Colocates key)
        {
            if (!base.ContainsKey(key))
                return;

            long nCount = base[key] - 1;
            if (nCount <= 0)
                base.Remove(key);
            else
                base[key] = nCount;

            Sum--;
            SquareSum = base.Values.Sum(v => v * v);
        }

        public virtual double Frequency(Colocates s)
        {
            if (Count == 0)
                return Double.NaN;

            long count;
            if (this.TryGetValue(s, out count))
                return Convert.ToDouble(count) / Sum;

            return 1.0 / Sum;
        }
        #endregion

        #region ISupportsStatistics Members
        public double TotalCount
        {
            get { return Convert.ToDouble(Count); }
        }

        public double Sum
        {
            get;
            protected set;
        }

        public double SquareSum
        {
            get;
            protected set;
        }

        public double Min
        {
            get;
            protected set;
        }

        public double Max
        {
            get;
            protected set;
        }
        #endregion

    }
}
