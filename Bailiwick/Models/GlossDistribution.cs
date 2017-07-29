using Esoteric.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Bailiwick.Models
{
    [Serializable]
    public class GlossDistribution : Dictionary<IGloss, long>, IDistribution<IGloss>, ISupportsStatistics
    {
        static GlossComparer _innerComparer = new GlossComparer();

        public GlossDistribution() : base(_innerComparer) { }
        protected GlossDistribution(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual IEnumerable<IGloss> Bins
        {
            get { return this.Keys; }
        }

        public virtual IGloss Mode
        {
            get
            {
                IGloss ret = null;
                long mode = 0;

                foreach (KeyValuePair<IGloss, long> pair in this)
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

        #region IDistribution<IGloss> Members

        public virtual void Increment(IGloss key)
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

        public virtual void Decrement(IGloss key)
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

        public virtual double Frequency(IGloss s)
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

        public IEnumerable<KeyValuePair<IGloss, long>> Sorted()
        {
            return this.OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key, _innerComparer);
        }
    }
}
