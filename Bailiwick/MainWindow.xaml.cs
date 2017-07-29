using Bailiwick.Models;
using Bailiwick.Parsers;
using Bailiwick.UI;
using Esoteric.UI;
using Esoteric.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Bailiwick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Analysis.ISentenceOperation, IProgressUI
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SelectedTab = 0;
        }

        public Analysis.SyntaxEngine TheEngine
        {
            get
            {
                return theEngine ?? (theEngine = new Analysis.SyntaxEngine(this, this));
            }
            set
            {
                theEngine = value;
            }
        }
        Analysis.SyntaxEngine theEngine;

        Dispatcher UIDispatcher
        {
            get
            {
                return Application.Current.Dispatcher;
            }
        }

        public ProgressViewModel Progress
        {
            get
            {
                return progress ?? (progress = new ProgressViewModel());
            }
            set
            {
                progress = value;
            }
        }
        ProgressViewModel progress;
	
        #region Command Properties

        public RelayCommand GoCommand
        {
            get
            {
                return goCommand ?? (goCommand = new RelayCommand(p => OnGo()));
            }
        }
        RelayCommand goCommand;

        public RelayCommand ResultPaneCopy
        {
            get
            {
                return resultPaneCopy ?? (resultPaneCopy = new RelayCommand(p => OnResultPaneCopy()));
            }
        }
        RelayCommand resultPaneCopy;

        public RelayCommand ReplacePaste
        {
            get
            {
                return replacePaste ?? (replacePaste = new RelayCommand(p => OnReplacePaste(), 
                                                                        p => CanReplacePaste()));
            }
        }
        RelayCommand replacePaste;

        public RelayCommand ScratchCommand
        {
            get
            {
                return scratchCommand ?? (scratchCommand = new RelayCommand(p => OnScratchCommand()));
            }
        }
        RelayCommand scratchCommand;

        #endregion

        #region Result Panes

        public ObservableCollection<IResultPane> ResultPanes
        {
            get
            {
                if (resultPanes == null)
                {
                    resultPanes = new ObservableCollection<IResultPane>();
                    //resultPanes.Add(new SentenceTypeFrequencyViewModel());
                    resultPanes.Add(new DecoratedSentenceViewModel());
                    resultPanes.Add(new WordFrequencyViewModel());
                    resultPanes.Add(new ColocatesViewModel());
                    resultPanes.Add(new KwicViewModel());
                    //resultPanes.Add(new PhraseNetworkViewModel());
                }
                return resultPanes;
            }
        }
        ObservableCollection<IResultPane> resultPanes;

        public int SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                selectedTab = value;

                if( TheToolTray.ToolBars.Count == 3 )
                    TheToolTray.ToolBars.RemoveAt(2);

                if (selectedTab >= 0 && selectedTab < ResultPanes.Count)
                {
                    var options = ResultPanes[selectedTab].Options;
                    if( options != null )
                        TheToolTray.ToolBars.Add(options);
                }
            }
        }
        int selectedTab;

        #endregion

        #region ISentenceOperation Members

        public void Process(Sentence s)
        {
            var a = new Action(() =>
            {
                foreach (var rp in ResultPanes)
                    rp.OnSentence(s);
            });

            UIDispatcher.BeginInvoke(a);
        }

        #endregion

        #region IProgressUI Members

        public void Beginning()
        {
            var a = new Action(() =>
            {
                foreach (var rp in ResultPanes)
                    rp.OnStartAnalysis(string.Empty);
            });

            UIDispatcher.BeginInvoke(a);

            Progress.Beginning();
        }

        public void SetMessage(string caption)
        {
            Progress.SetMessage(caption);
        }

        public void SetMinAndMax(int min, int max)
        {
            Progress.SetMinAndMax(min, max);
        }

        public void Increment()
        {
            Progress.Increment();
        }

        public void IncrementTo(int value)
        {
            Progress.IncrementTo(value);
        }

        public WaitHandle CancelEvent
        {
            get { return Progress.CancelEvent; }
        }

        public void Finished()
        {
            var a = new Action(() =>
            {
                foreach (var rp in ResultPanes)
                    rp.OnEndAnalysis();
            });

            UIDispatcher.BeginInvoke(a);

            Progress.Finished();
        }

        #endregion

        void OnGo()
        {
            var separators = new char[] {' ', '\t'};
            var tokens = SourceText.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            TheEngine.Tokenize = TokenizeFirst.IsChecked ?? false;

            var t = Task.Factory.StartNew(() => TheEngine.Run(tokens));
        }

        void OnResultPaneCopy()
        {
            ResultPanes[SelectedTab].OnCopy();
        }

        bool CanReplacePaste()
        {
            return Clipboard.ContainsText();
        }

        void OnReplacePaste()
        {
            SourceText.Clear();
            SourceText.Paste();
        }

        void OnScratchCommand()
        {
            var corpus = new Corpora.CocaCorpus();
            var sb = new StringBuilder();

            var brownCorpus = new Corpora.BrownCorpus();
            var comparer = new GlossComparer();


#if NOUNY
            var q1 = from a in corpus.OneGram.SelectMany(s => s)
                     where !a.GeneralWordClass.IsClosedWordClass()
                     group a by a.Lemma into b
                     select new 
                     {
                         Lemma = b.Key,
                         Adjectives = b.Where(g => g.GeneralWordClass == WordClassType.Adjective).Sum(g => g.Count),
                         Adverbs = b.Where(g => g.GeneralWordClass == WordClassType.Adverb).Sum(g => g.Count),
                         Nouns = b.Where(g => g.GeneralWordClass == WordClassType.Noun).Sum(g => g.Count),
                         Verbs = b.Where(g => g.GeneralWordClass == WordClassType.Verb).Sum(g => g.Count),
                         TotalSum = b.Sum(g => g.Count)
                     };

            sb.AppendLine("Lemma\tN-J\tN-V\tJ-V\tR-V\tJ-R");

            foreach(var q in q1.Where(g => g.TotalSum > 10))
            {
                double nj=0, nv=0, jv=0, rv=0, jr=0;

                double sum = q.Adjectives + q.Nouns;
                if( sum != 0 )
                    nj = (q.Adjectives - q.Nouns) / sum;

                sum = q.Verbs + q.Nouns;
                if (sum != 0)
                    nv = (q.Verbs - q.Nouns) / sum;

                sum = q.Verbs + q.Adjectives;
                if (sum != 0)
                    jv = (q.Verbs - q.Adjectives) / sum;

                sum = q.Verbs + q.Adverbs;
                if (sum != 0)
                    rv = (q.Verbs - q.Adverbs) / sum;

                sum = q.Adverbs + q.Adjectives;
                if (sum != 0)
                    jr = (q.Adjectives - q.Adverbs) / sum;

                sb.AppendFormat("{0}\t{1:N3}\t{2:N3}\t{3:N3}\t{4:N3}\t{5:N3}", q.Lemma, nj, nv, jv, rv, jr);
                sb.AppendLine();
            }
#endif

#if ATTEMPT_TO_EXTRACT_ROOTS
            //var aFreq = from a in corpus.OneGram.SelectMany(a => a.ToArray())
            //            ;
            var bFreq = from a in corpus.OneGram.SelectMany(a => a.ToArray())
                        where !a.IsClosedWordClass() && a.PartOfSpeech.General != WordClassType.Interjection
                        group a by a.Lemma.Substring(0,2) into r0
                        select r0;
            
            sb.AppendLine("Letter\tLemma\tNormalized\tPOS\tCount\tLength");

            RecurseSplit(bFreq, sb);
#endif

#if QA_ON_COCA
            var r0 = from a in corpus.OneGram.SelectMany(x => x.ToArray())
                     select new Summary 
                     {
                        Source = "COCA",
                        Normalized = a.Normalized,
                        General = a.PartOfSpeech.General,
                        Specific = a.PartOfSpeech.Specific,
                        Count = a.Count
                     };

            var r1 = from a in brownCorpus.OneGram.SelectMany(x => x.ToArray())
                     where a.Count > 1
                        && !a.PartOfSpeech.Specific.StartsWith("NP") 
                        && !a.Normalized.StartsWith(".")
                        && a.PartOfSpeech.General != WordClassType.Interjection
                        && a.PartOfSpeech.General != WordClassType.Unclassified
                     select new Summary
                     {
                        Source = "Brown",
                        Normalized = a.Normalized,
                        General = a.PartOfSpeech.General,
                        Specific = a.PartOfSpeech.Specific,
                        Count = a.Count
                     };

            var sc = new SummaryComparer();
            //var mb = r0.Except(r1, sc).ToArray();
            var mc = r1.Except(r0, sc).ToArray();

            var wordSet = mc.Select(x => x.Normalized).Distinct().ToArray();

            var sameSet = r0.Union(r1).Join(wordSet, ok => ok.Normalized, ik => ik, (o,i) => o);

            var output = sameSet.OrderBy(x => x.Normalized).ThenBy(x => x.Specific).ThenBy(x => x.Source).ToArray();

            sb.AppendLine("Normalized\tSpecific\tGeneral\tSource\tCount");
            foreach (var s in output)
            {
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}", s.Normalized, s.Specific, s.General, s.Source, s.Count);
                sb.AppendLine();
            }
#endif
            var r0 = from a in corpus.OneGram.SelectMany(x => x.ToArray())
                     where a.PartOfSpeech.General == WordClassType.Adjective
                     select a;

            var r1 = from a in corpus.OneGram.SelectMany(x => x.ToArray())
                     where a.PartOfSpeech.General == WordClassType.Adverb
                     select a;

            var sameSet = r0.Join(r1, ok => ok.Normalized, ik => ik.Normalized, (o, i) => o);

            var output = sameSet.OrderBy(x => x.Normalized).ThenBy(x => x.PartOfSpeech.Specific).ToArray();

            sb.AppendLine("Normalized");
            foreach (var s in output)
            {
                sb.Append(s.Normalized);
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        void RecurseSplit(IEnumerable<IGrouping<string, Frequency>> bFreq, StringBuilder sb)
        {
            int count = bFreq.Count();

            if( count == 0 )
                return;

            if (count == 1)
            {
                Handle(bFreq.First(), sb);
                return;
            }

            foreach (var d0 in bFreq.OrderBy(x => x.Key))
            {
                // if d0.key starts with
                // hyper, super, mega, ultra
                // un, under
                // in, inter
                // high-, higher-
                // well-, good-, bad-
                // half-, full-, all-
                // anti-
                // white-, black-, hard-

                var minLen = d0.Min(x => x.Lemma.Length);

                if (minLen > 2)
                {
                    var b1 = from a in d0
                             group a by a.Lemma.Substring(0, minLen) into r1
                             select r1;

                    RecurseSplit(b1, sb);
                }

                else
                {
                    Handle(d0, sb);
                }
            }
        }

        void Handle(IGrouping<string, Frequency> d0, StringBuilder sb)
        {
            foreach (var d in d0.OrderBy(x => x.Lemma))
            {
                sb.AppendFormat("{4}\t{5}\t{0}\t{1}\t{2}\t{3}", d.Normalized,
                                                      d.PartOfSpeech.Specific,
                                                      d.Count, d.Normalized.Length, d0.Key, d.Lemma);
                sb.AppendLine();
            }
        }

    }

    internal class Summary
    {
      internal string Source {get; set; }
      internal string Normalized { get; set; }
      internal WordClassType General { get; set; }
      internal string Specific { get; set; }
      internal long Count { get; set; }
    }

    internal class SummaryComparer : IEqualityComparer<Summary>
    {
        public bool Equals(Summary x, Summary y)
        {
            return x.Normalized == y.Normalized && x.Specific == y.Specific;
        }

        public int GetHashCode(Summary obj)
        {
            return obj.Normalized.GetHashCode() ^ obj.Specific.GetHashCode();
        }
    }

}
