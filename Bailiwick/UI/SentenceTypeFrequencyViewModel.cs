using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using Esoteric.UI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    public class SentenceTypeDisplay
    {
        public int Index { get; set; }
        public string Pronouns { get; set; }
        public string Adverbs { get; set; }
        public string Phrases { get; set; }
        public string Sentence { get; set; }
    }

    public class SentenceTypeFrequencyViewModel : ViewModelBase, IResultPane
    {
        public SentenceTypeFrequencyViewModel()
        {
            View = new SentenceTypeFrequencyView { Model = this };
        }

        #region Binding Properties

        public ObservableCollection<SentenceTypeDisplay> Distribution
        {
            get
            {
                return distribution ?? (distribution = new ObservableCollection<SentenceTypeDisplay>());
            }
        }
        ObservableCollection<SentenceTypeDisplay> distribution;
	
        #endregion

        #region IResultPane Members

        public ToolBar Options { get; set; }

        public string Title
        {
            get
            {
                return title ?? (title = "Sentence Types");
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
            Distribution.Clear();
        }

        public void OnSentence(Sentence s)
        {
            if (s.Length == 0)
                return;

            if (s.GetAll<ISentenceNode>().Count() == 0 && s.Words.All(w => string.IsNullOrWhiteSpace(w.Instance)))
                return;

            var sb = new StringBuilder();
            var current = s[0].FirstOrDefault();
            while( current != null )
            {
                var phrase = current as IPhrase;
                var nphrase = current as NounPhrase;
                if (phrase != null)
                {
                    switch (phrase.GeneralWordClass)
                    {
                        case WordClassType.Adjective:
                            sb.Append("JP ");
                            break;

                        case WordClassType.Adverb:
                            sb.Append("RP ");
                            break;

                        case WordClassType.Noun:
                            if (nphrase != null && nphrase.IsPrepositionalPhrase)
                                sb.Append("PP ");
                            else if( nphrase != null )
                                sb.Append("NP ");
                            else
                                sb.Append("SP ");
                            break;

                        case WordClassType.Verb:
                            sb.Append("VP ");
                            break;

                        case WordClassType.Conjunction:
                            sb.Append("CP ");
                            break;

                        default:
                            sb.Append("?P");
                            break;
                    }

                    current = s.Next<ISentenceNode>(phrase).FirstOrDefault();
                }

                else if (!string.IsNullOrWhiteSpace(current.Head.Instance))
                {
                    sb.AppendFormat("{0} ", current.Head.PartOfSpeech.Specific);
                    current = s.Next<ISentenceNode>(current).FirstOrDefault();
                }

                else
                {
                    current = s.Next<ISentenceNode>(current).FirstOrDefault();
                }
            }
            
            // Analyzing adverbs and pronouns
            var pronouns = (from a in s.Words
                           where a.GeneralWordClass == WordClassType.Pronoun && a.PartOfSpeech.Specific == "PN1"
                           orderby a.Instance
                           select a.Normalized).Distinct().ToArray();

            var adverbs = (from a in s.Words
                          where a.GeneralWordClass == WordClassType.Adverb
                          orderby a.Instance
                          select a.Normalized).Distinct().ToArray();

//            if (pronouns.Length + adverbs.Length == 0 )
//                return;

            // Make the row
            var row = new SentenceTypeDisplay
            {
                Index = s.ID,
                Adverbs = string.Join(", ", adverbs),
                Phrases = sb.ToString(),
                Pronouns = string.Join(", ", pronouns),
                Sentence = string.Join(" ", s.Words.Where(x => !string.IsNullOrWhiteSpace(x.Instance)).Select(x => x.Instance))
            };

            if (string.IsNullOrWhiteSpace(row.Phrases))
                return;

            Distribution.Add(row);
        }

        public void OnEndAnalysis()
        {
        }

        public void OnCopy()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Index\tPhrases\tPronouns\tAdverbs\tSentence");

            foreach (var d in Distribution)
            {
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}", d.Index
                                                                   , d.Phrases
                                                                   , d.Pronouns
                                                                   , d.Adverbs
                                                                   , d.Sentence);
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        #endregion
    }
}
