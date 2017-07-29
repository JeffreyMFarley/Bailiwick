using Bailiwick.Models;
using Esoteric.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    public class FrequencyDisplay
    {
        public Int64 Count { get; set; }
        public string Lemma { get; set; }
        public string Normalized { get; set; }
        public WordClassType General { get; set; }
        public string Specific { get; set; }
        public bool IsOpenClass { get; set; }
    }

    public class WordFrequencyViewModel : ViewModelBase, IResultPane
    {
        public WordFrequencyViewModel()
        {
            View = new WordFrequencyView { Model = this };
            Options = new WordFrequencyOptions { Model = this };
        }

        #region Data Binding Properties

        public IEnumerable<FrequencyDisplay> Distribution
        {
            get
            {
                Func<KeyValuePair<IGloss, long>, bool> predicate = (x => !x.Key.IsClosedWordClass() || ShowAll);

                long totalCount = RawDistribution.Where(predicate).Sum(x => x.Value);
                long maxCount =  (DistributionPercentage * totalCount) / 100;
                long accum = 0;

                foreach (var d in RawDistribution.Sorted())
                {
                    var wi = d.Key;
                    var boring = wi.IsClosedWordClass();

                    if (predicate(d) && accum < maxCount)
                    {
                        accum += d.Value;

                        yield return new FrequencyDisplay
                        {
                            Count = d.Value,
                            General = wi.PartOfSpeech.General,
                            Lemma = wi.Lemma,
                            Normalized = wi.Normalized,
                            Specific = wi.PartOfSpeech.Specific,
                            IsOpenClass = !wi.IsClosedWordClass()
                        };
                    }
                }
            }
        }

        public bool ShowAll
        {
            get
            {
                return showAll;
            }
            set
            {
                showAll = value;
                OnPropertyChanged("ShowAll");
                OnPropertyChanged("Distribution");
            }
        }
        bool showAll;

        public long DistributionPercentage
        {
            get
            {
                return distributionPercentage;
            }
            set
            {
                distributionPercentage = value;
                OnPropertyChanged("DistributionPercentage");
                OnPropertyChanged("Distribution");
            }
        }
        long distributionPercentage = 80;
	
        #endregion

        #region Unbound (Local) Properties

        public GlossDistribution RawDistribution
        {
            get
            {
                if (rawDistribution == null)
                {
                    rawDistribution = new GlossDistribution();
                }
                return rawDistribution;
            }
        }
        GlossDistribution rawDistribution;
        
        #endregion

        #region IResultPane Members

        public string Title
        {
            get
            {
                return title ?? (title = "Word Frequency");
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
        string title;

        public ToolBar Options
        {
            get;
            private set;
        }

        public void OnStartAnalysis(string documentName)
        {
            RawDistribution.Clear();
        }

        public void OnSentence(Sentence s)
        {
            foreach(var w in s.Words)
              RawDistribution.Increment(w);
        }

        public void OnEndAnalysis()
        {
            OnPropertyChanged("Distribution");
        }

        public void OnCopy()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Normalized\tLemma\tGeneral\tSpecific\tCount\tIsOpen");

            foreach (var d in Distribution)
            {
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", d.Normalized, d.Lemma, d.General, d.Specific, d.Count, d.IsOpenClass);
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        #endregion
    }
}
