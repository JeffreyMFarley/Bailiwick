using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Esoteric.Collections;

namespace Bailiwick.Models
{
    public class Sentence : IIntervalProvider<int>
    {
        public Sentence()
        {
            ID = AutoIndex();
        }

        #region Class Properties

        static SentenceNodeComparer NodeComparer
        {
            get
            {
                if (nodeComparer == null)
                {
                    nodeComparer = new SentenceNodeComparer();
                }
                return nodeComparer;
            }
        }
        static SentenceNodeComparer nodeComparer;

        static object _lock = new object();

        static int AutoIndex()
        {
            int result = 0;
            lock (_lock)
            {
                _id++;
                result = _id;
            }
            return result;
        }
        static int _id = 0;

        static public void ResetIndex()
        {
            lock (_lock)
            {
                _id = 0;
            }
        }

        #endregion

        #region IIntervalProvider<int> Members

        public int StartIndex
        {
            get { return 0; }
        }

        public int EndIndex
        {
            get { return Length - 1; }
        }

        #endregion

        public int ID { get; private set; }

        public int Length
        {
            get;
            private set;
        }

        public SyntaxScan GeneralSyntaxProperties
        {
            get
            {
                return SyntaxScan.Create(this);
            }
        }

        public override int GetHashCode()
        {
            int accum = 0;
            for (int i = 0; i < words.Count; i++)
                accum ^= words[i].GetHashCode();

            return accum;
        }

        #region Data Structures

        List<WordInstance> words = new List<WordInstance>();

        SegmentTree<ISentenceNode> Segments
        {
            get
            {
                if (segments == null)
                {
                    segments = new SegmentTree<ISentenceNode>();
                }
                return segments;
            }
        }
        SegmentTree<ISentenceNode> segments;

        #endregion

        #region Navigation Methods

        public IReadOnlyList<WordInstance> Words
        {
            get { return words; }
        }

        public IEnumerable<T> GetAll<T>()
            where T : class, ISentenceNode
        {
            var q = Segments.Union(Words).OfType<T>().OrderBy(x => x, NodeComparer);
            foreach (var x in q)
                yield return x;
        }

        public IEnumerable<ISentenceNode> this[int index]
        {
            get
            {
                foreach (var s in Segments.FindInRange(index, index).OrderBy(x => x, NodeComparer))
                    yield return s;

                if( index >= StartIndex && index <= EndIndex )
                    yield return Words[index];
            }
        }

        public IEnumerable<T> Previous<T>(T i)
            where T : class, ISentenceNode
        {
            if( i == null )
                yield break;

            foreach(var n in this[i.StartIndex  - 1].OfType<T>())
                yield return n;
        }

        public IEnumerable<T> Next<T>(T i)
            where T : class, ISentenceNode
        {
            if (i == null)
                yield break;

            foreach (var n in this[i.EndIndex + 1].OfType<T>())
                yield return n;
        }

        #endregion

        #region Mutating Methods

        public void Add(WordInstance w)
        {
            w.StartIndex = Length;
            Length += 1;
            words.Add(w);
        }

        public void Add(ISentenceNode n)
        {
            Segments.Add(n);
        }

        public void Remove(ISentenceNode n)
        {
            Segments.Remove(n);
        }

        public void Refresh(ISentenceNode n)
        {
            Segments.Remove(n);
            Segments.Add(n);
        }

        public void AddRange(IList<IPhrase> pl)
        {
            foreach (var p in pl)
                Segments.Add(p);
        }

        public void Trim(IPhrase n, WordInstance word)
        {
            Segments.Remove(n);
            n.Remove(word);
            Segments.Add(n);
        }

        public void Trim(IPhrase n, int start, int end)
        {
            Segments.Remove(n);
            for (int i = start; i <= end; i++)
                n.Remove(Words[i]);
            Segments.Add(n);
        }
        #endregion

    }
}
