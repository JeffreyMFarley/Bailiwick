using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Bailiwick.Models;
using Esoteric.UI;

namespace Bailiwick.UI
{
    public class KwicViewModel : ViewModelBase, IResultPane
    {
        public KwicViewModel()
        {
            View = new KwicView { Model = this };
            Options = new KwicOptions { Model = this };   
        }

        #region Data Binding Properties

        public ObservableCollection<IKwicStrategy> AvailableGroupings
        {
            get
            {
                if (availableGroupings == null)
                {
                    availableGroupings = new ObservableCollection<IKwicStrategy>();
                    availableGroupings.Add(new Strategies.KwicWord());
                    availableGroupings.Add(new Strategies.KwicLemma());
                    availableGroupings.Add(new Strategies.KwicWordClassType(WordClassType.Noun));
                    availableGroupings.Add(new Strategies.KwicWordClassType(WordClassType.Verb));
                    availableGroupings.Add(new Strategies.KwicProcessVerb());
                }
                return availableGroupings;
            }
        }
        ObservableCollection<IKwicStrategy> availableGroupings;

        public IKwicStrategy SelectedGrouping
        {
            get
            {
                return selectedGrouping ?? (selectedGrouping = AvailableGroupings[0]);
            }
            set
            {
                selectedGrouping = value;
                OnPropertyChanged("SelectedGrouping");

                SelectedKeyWord = null;
                OnPropertyChanged("AvailableKeyWords");
            }
        }
        IKwicStrategy selectedGrouping;

        public IEnumerable<string> AvailableKeyWords
        {
            get
            {
                if( SelectedGrouping == null )
                    yield break;

                var q = SelectedGrouping.KeyWords(KeyWords);
                foreach (var w in q.Where(x => !string.IsNullOrWhiteSpace(x))
                                   .Distinct())
                    if( !string.IsNullOrEmpty(w) )
                        yield return w;
            }
        }

        public string SelectedKeyWord
        {
            get
            {
                return selectedKeyWord ?? (selectedKeyWord = string.Empty);
            }
            set
            {
                selectedKeyWord = value;
                OnPropertyChanged("SelectedKeyWord");
                OnPropertyChanged("KwicTable");
            }
        }
        string selectedKeyWord;

        public IEnumerable<KwicDisplay> KwicTable
        {
            get
            {
                if( string.IsNullOrEmpty(SelectedKeyWord) )
                    yield break;

                var m = SelectedGrouping.BuildFilter(SelectedKeyWord);
                foreach (var s in Sentences.Where(s => s.Words.Any(m)))
                {
                    // Find the matching index
                    foreach (var w in s.Words.Where(m))  
                        yield return new KwicDisplay
                        {
                            Sentence = s.ID,
                            Word = w.StartIndex,
                            Before = string.Join(" ", s.Words.Where(x => x.StartIndex < w.StartIndex 
                                                                      && !string.IsNullOrWhiteSpace(x.Normalized))
                                                             .Select(x => x.Instance)),
                            KeyWord = w.Instance,
                            After = string.Join(" ", s.Words.Where(x => x.StartIndex > w.StartIndex
                                                                     && !string.IsNullOrWhiteSpace(x.Normalized))
                                                            .Select(x => x.Instance))
                        };
                }
            }
        }

        #endregion
        #region Unbound (Local) Properties

        GlossDistribution KeyWords
        {
            get
            {
                if (keyWords == null)
                {
                    keyWords = new GlossDistribution();
                }
                return keyWords;
            }
        }
        GlossDistribution keyWords;

        List<Sentence> Sentences
        {
            get
            {
                if (sentences == null)
                {
                    sentences = new List<Sentence>();
                }
                return sentences;
            }
        }
        List<Sentence> sentences;

        public WordInstance NullInstance
        {
            get
            {
                if (nullInstance == null)
                {
                    nullInstance = new WordInstance(string.Empty);
                }
                return nullInstance;
            }
        }
        WordInstance nullInstance;
	
        #endregion

        #region IResult Pane Members

        public ToolBar Options { get; private set; }

        public string Title
        {
            get
            {
                return title ?? (title = "Key Word in Context");
            }
            set
            {
                title = value;
            }
        }
        string title;
	
        public void OnStartAnalysis(string documentName)
        {
            KeyWords.Clear();
            Sentences.Clear();
            SelectedKeyWord = null;
        }

        public void OnSentence(Sentence s)
        {
            foreach (var w in s.Words.Where(x => !x.IsClosedWordClass()) )
                KeyWords.Increment(w);

            Sentences.Add(s);
        }

        public void OnEndAnalysis()
        {
            OnPropertyChanged("AvailableKeyWords");
            OnPropertyChanged("SelectedKeyWord");
            OnPropertyChanged("KwicTable");
        }

        public void OnCopy()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Before\tKeyWord\tAfter");

            foreach (var d in KwicTable)
            {
                sb.Append(string.Join("\t", d.Before, d.KeyWord, d.After));
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        #endregion
    }

    public class KwicDisplay
    {
        public int Sentence { get; set; }
        public int Word { get; set; }
        public string Before { get; set; }
        public string KeyWord { get; set; }
        public string After { get; set; }
    }
}
