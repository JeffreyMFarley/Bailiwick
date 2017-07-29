using Bailiwick.Models;
using Esoteric.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    public class ColocateDisplay
    {
        public string Phrase { get; set; }
        public Int64 Count { get; set; }
        public string PhraseType { get; set; }
        public string HeadLemma { get; set; }
    }

    public class ColocatesViewModel : ViewModelBase, IResultPane
    {
        public ColocatesViewModel()
        {
            View = new ColocatesView { Model = this };
            Options = new ColocatesOptions { Model = this };
        }

        #region Data Binding Properties

        public IEnumerable<ColocateDisplay> Distribution
        {
            get
            {
                var collection = (IncludeAdjuncts) ? RawAdjunctColocates : RawClassColocates;

                var l = from r in collection
                        where r.Key.Length >= MinimumColocateLength
                        orderby r.Value descending, r.Key
                        select r;

                long totalCount = l.Sum(x => x.Value);
                long maxCount = (DistributionPercentage * totalCount) / 100;
                long accum = 0;

                foreach (var c in l)
                {
                    string headLemma = string.Empty;
                    if (c.Key.Head != null)
                    {
                        headLemma = (string.IsNullOrEmpty(c.Key.Head.Lemma)) ? c.Key.Head.Normalized : c.Key.Head.Lemma;
                    }

                    if( accum > maxCount )
                        yield break;

                    accum += c.Value;

                    yield return new ColocateDisplay
                    {
                        Count = c.Value,
                        Phrase = c.Key.ToString(),
                        PhraseType = c.Key.WordClassType.ToString(),
                        HeadLemma = headLemma
                    };
                }
            }
        }

        public bool IncludeAdjuncts
        {
            get
            {
                return includeAdjuncts;
            }
            set
            {
                includeAdjuncts = value;
                OnPropertyChanged("IncludeAdjuncts");
                OnPropertyChanged("Distribution");
            }
        }
        bool includeAdjuncts;

        public int MinimumColocateLength
        {
            get
            {
                return minimumColocateLength;
            }
            set
            {
                minimumColocateLength = value;
                OnPropertyChanged("MinimumColocateLength");
                OnPropertyChanged("Distribution");
            }
        }
        int minimumColocateLength = 2;

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

        #region Non-Bound (Local) Properties

        public ColocateDistribution RawClassColocates
        {
            get
            {
                if (rawClassColocates == null)
                {
                    rawClassColocates = new ColocateDistribution();
                }
                return rawClassColocates;
            }
        }
        ColocateDistribution rawClassColocates;

        public ColocateDistribution RawAdjunctColocates
        {
            get
            {
                if (rawAdjunctColocates == null)
                {
                    rawAdjunctColocates = new ColocateDistribution();
                }
                return rawAdjunctColocates;
            }
        }
        ColocateDistribution rawAdjunctColocates;
	
        #endregion
        
        #region IResultPane Members

        public ToolBar Options
        {
            get;
            private set;
        }

        public string Title
        {
            get
            {
                return title ?? (title = "Colocates");
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
        string title;

        public void OnStartAnalysis(string documentName)
        {
            RawClassColocates.Clear();
            RawAdjunctColocates.Clear();
        }

        public void OnSentence(Sentence s)
        {
            IEnumerable<Colocates> l;
            
            l = from a in s.GetAll<IPhrase>()
                where a.AdjunctColocates.Count > 0
                select new Colocates(a.AdjunctColocates, a.GeneralWordClass, a.Head);

            foreach (var c in l)
                RawAdjunctColocates.Increment(c);
                
            l = from a in s.GetAll<IPhrase>()
                where a.ClassColocates.Count > 0
                select new Colocates(a.ClassColocates, a.GeneralWordClass, a.Head);

            foreach (var c in l)
                RawClassColocates.Increment(c);
        }

        public void OnEndAnalysis()
        {
            OnPropertyChanged("Distribution");
        }

        public void OnCopy()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Phrase\tCount\tPhrase Type\tHeadLemma");
            foreach (var c in Distribution)
            {
                sb.AppendFormat(string.Join("\t", c.Phrase, c.Count, c.PhraseType, c.HeadLemma));
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        #endregion

    }
}
