using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;
using Esoteric.Collections;
using GlossPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.IGloss>>;
using WordClassPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.WordClass>>;

namespace Bailiwick.Morphology
{
    public class Classifier : IComparer<ClassifierNode>
    {
        static readonly TraceSource _ts = TraceSources.Classifier;
        static public StringDistribution PredicateTally = new StringDistribution();

        public Classifier(ICorpus corpus)
        {
            Corpus = corpus;

            Reset();
        }

        #region Properties

        public WordInstance StartOfSentence
        {
            get
            {
                return startOfSentence ?? (startOfSentence = new WordInstance("", WordClasses.BeforeClauseMarker));
            }
        }
        WordInstance startOfSentence;

        WordInstance PreviousWord { get; set; }
        int MostRecentIndex { get; set; }

        ICorpus Corpus { get; set; }

        WordClassPartials UnknownPartials
        {
            get
            {
                if (unknownPartials == null)
                {
                    unknownPartials = new List<Percentage<WordClass>>();

                    foreach (var o in Corpus.OpenClassRatios)
                    {
                        WordClass wc = null;
                        switch (o.Value)
                        {
                            case WordClassType.Adjective:
                                wc = WordClasses.Adjective;
                                break;

                            case WordClassType.Adverb:
                                wc = WordClasses.Adverb;
                                break;

                            case WordClassType.Noun:
                                wc = WordClasses.Noun;
                                break;

                            case WordClassType.Verb:
                                wc = WordClasses.Verb;
                                break;
                        }

                        if (wc != null)
                            unknownPartials.Add(new Percentage<WordClass>(wc, o.Partial));
                    }
                }
                return unknownPartials;
            }
        }
        WordClassPartials unknownPartials;

        WordClass ProperNoun
        {
            get
            {
                if (properNoun == null)
                {
                    properNoun = WordClasses.Specifics["NP1"];
                }
                return properNoun;
            }
        }
        WordClass properNoun;

        WordClass Acronym
        {
            get
            {
                if (acronym == null)
                {
                    acronym = WordClasses.Specifics["NP1"];
                }
                return acronym;
            }
        }
        WordClass acronym;

        List<ClassifierNode> Leaves
        {
            get { return leaves ?? (leaves = new List<ClassifierNode>()); }
        }
        List<ClassifierNode> leaves;

        WordCaseStatus CaseStatus
        {
            get
            {
                return caseStatus ?? (caseStatus = new WordCaseStatus());
            }
        }
        WordCaseStatus caseStatus;

        GlossComparer GlossComparer
        {
            get
            {
                return glossComparer ?? (glossComparer = new GlossComparer());
            }
        }
        GlossComparer glossComparer;

        #endregion
        #region Public Methods

        public IEnumerable<Sentence> Process(IEnumerable<Thought> thoughts)
        {
            foreach (var t in thoughts)
            {
                Reset();
                foreach (var wi in t.Words)
                {
                    // Handle the word
                    ProcessWord(wi);

                    // Too many cooks, reduce
                    if (Leaves.Count > 500)
                        Reduce();

                    PreviousWord = wi;
                }

                yield return GetResults();
            }
        }

        #endregion
        #region Internal Methods

        void Reset()
        {
            _ts.TraceEvent(TraceEventType.Verbose, 1, "Reset");

            Leaves.Clear();
            MostRecentIndex = 0;
            PreviousWord = StartOfSentence;
        }

        void ProcessWord(WordInstance wi)
        {
            MostRecentIndex += 1;

            // A new clause is being introduced, take the opportunity to reduce
            if (wi.PartOfSpeech == WordClasses.Semicolon ||
                wi.PartOfSpeech == WordClasses.Hyphen)
                Reduce();

            // No need to classify, it is already done
            if (wi.GeneralWordClass != WordClassType.Unclassified)
            {
                Add(wi.Instance, wi.PartOfSpeech);
                return;
            }

            // Apply the test
            var partials = Corpus.Classifications(wi);
            if (partials.Count == 0)
            {
                CaseStatus.Check(wi.Instance);
                if (wi.Instance.Length == 2 && char.IsLetter(wi.Instance[0]) && wi.Instance[1] == '.')
                {
                    Add(wi.Instance, (CaseStatus.UpperCase ?? false) ? ProperNoun : WordClasses.Letter);
                }
                else
                {
                    if (CaseStatus.UpperCase ?? false)
                        Add(wi.Instance, Acronym);
                    else if (CaseStatus.TitleCase ?? false)
                        Add(wi.Instance, ProperNoun);
                    else
                        AddUnknown(wi.Instance);
                }
            }
            else
                Add(wi.Instance, partials);
        }

        IEnumerable<ClassifierNode> Sort()
        {
            return Leaves.OrderByDescending(x => x, this);
        }

        void Reduce()
        {
            if (Leaves.Count < 2)
                return;

            _ts.TraceEvent(TraceEventType.Verbose, 4, "Reduce");

            var winner = Sort().Take(10).ToArray();
            Leaves.Clear();
            Leaves.AddRange(winner);
        }

        Sentence GetResults()
        {
            _ts.TraceEvent(TraceEventType.Information, 3, "GetResults: {0} leaves", Leaves.Count);

            if (_ts.Switch.ShouldTrace(TraceEventType.Verbose))
            {
                var l = Leaves.OrderByDescending(x => x, this);
                var stk = new Stack<string>();
                foreach (var branch in l)
                {
                    var cur = branch; stk.Clear();
                    do
                    {
                        stk.Push(cur.Word.PartOfSpeech.Specific);
                        cur = cur.Root;
                    } while (cur != null);

                    var line = string.Join(" ", stk);
                    _ts.TraceEvent(TraceEventType.Verbose, 3, "[{0}] {1}", branch.Score, line);
                }
            }

            var sentence = new Sentence();

            if (Leaves.Count == 0)
                return sentence;

            ClassifierNode winner = Sort().FirstOrDefault();
            if (winner == null)
            {
                winner = Leaves.OrderByDescending(x => x, this).First();
            }
            var stack = new Stack<WordInstance>();
            do
            {
                stack.Push(winner.Word);
                winner = winner.Root;
            } while (winner != null);

            while (stack.Count != 0)
            {
                sentence.Add(stack.Pop());
            }

            Reset();

            return sentence;
        }

        #endregion
        #region Upbuilding Methods

        protected void Add(string instance, GlossPartials partials)
        {
            Add(instance, partials, WordInstance.CreateWithGloss);
        }

        protected void Add(string instance, WordClass theWordClass)
        {
            var asPartial = new Percentage<WordClass>(theWordClass, 1.0);
            Add(instance, Enumerable.Repeat(asPartial, 1), WordInstance.CreateWithWordClass);
        }

        protected void AddUnknown(string instance)
        {
            Add(instance, UnknownPartials, WordInstance.CreateWithWordClass);
        }

        protected void Add<T>(string instance, IEnumerable<Percentage<T>> wcs, Func<string, T, WordInstance> creator)
        {
            int expected = 0;

            if (Leaves.Count == 0)
            {
                expected = wcs.Count();
                _ts.TraceEvent(TraceEventType.Verbose, 2, "Adding {1} instances of '{0}' to an empty array", instance, expected);

                foreach (var wc in wcs)
                {
                    var wi = creator(instance, wc.Value);
                    Leaves.Add(new ClassifierNode(wi, wc.Partial));
                }
            }
            else
            {
                expected = wcs.Count() * Leaves.Count;
                _ts.TraceEvent(TraceEventType.Verbose, 2, "Adding {1} instances of '{0}' to {2} leaves", instance, wcs.Count(), Leaves.Count);

                var previousWord = Leaves[0].Word.Instance;
                var list = new List<ClassifierNode>(Leaves);
                Leaves.Clear();

                foreach (var n in list)
                {
                    foreach (var wc in wcs)
                    {
                        var wi = creator(instance, wc.Value);
                        if (n.AcceptAsNext(wi, wc.Partial))
                            Leaves.Add(new ClassifierNode(n, wi, wc.Partial));
                        else
                            _ts.CompressedTraceEvent(TraceEventType.Verbose, 2, "Rejected {0} => {1}", n.Word, wi);
                    }
                }

                if (Leaves.Count == 0) 
                { 
                    _ts.TraceEvent(TraceEventType.Error, 2, "All leaves rejected: '{0}' => '{1}'", previousWord, instance);

                    foreach (var n in list)
                    {
                        WordClass wc = WordClasses.Specifics["FU"];
                        if (typeof(T) == typeof(IGloss) && wcs.Count() == 1)
                            wc = (wcs.First().Value as IGloss).PartOfSpeech;

                        var wi = new WordInstance(instance, wc);
                        Leaves.Add(new ClassifierNode(n, wi, 1.0));
                    }
                }
            }

            _ts.TraceEvent(TraceEventType.Verbose, 2, "'{0}' resulted in {1} leaves vs {2}", instance, Leaves.Count, expected);
        }
        
        #endregion
        #region IComparer<ClassifierNode> Members

        public int Compare(ClassifierNode x, ClassifierNode y)
        {
            //return GlossComparer.Compare(x.Word,y.Word);
            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x == null && y == null)
                return 0;

            //var xScan = x.ScanBuilder.Result;
            //var yScan = y.ScanBuilder.Result;

            //if (!xScan.IsComplete && yScan.IsComplete)
            //    return -1;

            //if (xScan.IsComplete && !yScan.IsComplete)
            //    return 1;

            return x.Score.CompareTo(y.Score);
        }

        #endregion
    }
}

