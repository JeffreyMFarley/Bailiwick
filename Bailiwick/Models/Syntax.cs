using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Esoteric.Collections;
using SyntaxPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.WordClassType>>;

namespace Bailiwick.Models
{
    public class Syntax : Dictionary<SyntaxPair, long>, ISyntax, IDistribution<SyntaxPair>, ISupportsStatistics
    {
        public Syntax() : base() { }
        protected Syntax(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual IEnumerable<SyntaxPair> Bins
        {
            get { return this.Keys; }
        }

        public virtual SyntaxPair Mode
        {
            get
            {
                SyntaxPair ret = null;
                long mode = 0;

                foreach (KeyValuePair<SyntaxPair, long> pair in this)
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

        public void OnLoaded()
        {
            Sum = base.Values.Sum();
            SquareSum = base.Values.Sum(v => v * v);
        }

        #region ISyntax Methods

        SyntaxPartials EmptyPartials
        {
            get
            {
                return emptyPartials ?? (emptyPartials = new List<Percentage<WordClassType>>());
            }
        }
        SyntaxPartials emptyPartials;

        Dictionary<WordClassType, SyntaxPartials> PartialsCache
        {
            get
            {
                if (partialsCache == null)
                {
                    partialsCache = new Dictionary<WordClassType, SyntaxPartials>();
                }
                return partialsCache;
            }
        }
        Dictionary<WordClassType, SyntaxPartials> partialsCache;

        Dictionary<WordClassType, double> PreviousProbability
        {
            get
            {
                if (previousProbability == null)
                {
                    previousProbability = new Dictionary<WordClassType, double>();
                }
                return previousProbability;
            }
        }
        Dictionary<WordClassType, double> previousProbability;

        Dictionary<WordClassType, double> NextProbability
        {
            get
            {
                if (nextProbability == null)
                {
                    nextProbability = new Dictionary<WordClassType, double>();
                }
                return nextProbability;
            }
        }
        Dictionary<WordClassType, double> nextProbability;
	
        public void Increment(IGloss previous, IGloss next)
        {
            var gposPrev = (previous != null) ? previous.PartOfSpeech.General : WordClassType.Start;
            var gposNext = (next != null) ? next.PartOfSpeech.General : WordClassType.Unclassified;

            if (gposPrev == WordClassType.Unclassified || gposNext == WordClassType.Unclassified)
                return;

            Increment(new SyntaxPair(gposPrev, gposNext));
        }

        public double PercentNextGivenPrevious(WordClassType previous, WordClassType next)
        {
            var q = PreviousPartials(previous).FirstOrDefault(a => a.Value == next);
            return (q == null) ? 0.0 : q.Partial;
        }

        public SyntaxPartials PreviousPartials(WordClassType previous)
        {
            if (PartialsCache.ContainsKey(previous))
                return PartialsCache[previous];

            var q = from a in this
                    where a.Key.Previous == previous
                    select a;

            double sum = q.Sum(a => a.Value);
            if( sum == 0 )
                return EmptyPartials;

            var r =  q.Select(a => new Percentage<WordClassType>(a.Key.Next, a.Value / sum))
                      .ToArray();

            PartialsCache.Add(previous, r);
            return r;
        }

        public double PercentPrevious(WordClassType previous)
        {
            if (PreviousProbability.ContainsKey(previous))
                return PreviousProbability[previous];

            var r = this.Where(a => a.Key.Previous == previous).Sum(x => x.Value) / Sum;
            PreviousProbability.Add(previous, r);
            return r;
        }

        public double PercentNext(WordClassType next)
        {
            if (NextProbability.ContainsKey(next))
                return NextProbability[next];

            var r = this.Where(a => a.Key.Next == next).Sum(x => x.Value) / Sum;
            NextProbability.Add(next, r);
            return r;
        }
        
        #endregion

        #region IDistribution<SyntaxPair> Members

        public virtual void Increment(SyntaxPair key)
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

        public virtual void Decrement(SyntaxPair key)
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

        public virtual double Frequency(SyntaxPair s)
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
