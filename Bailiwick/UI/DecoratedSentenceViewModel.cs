using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using Esoteric.UI;

namespace Bailiwick.UI
{
    internal class WordClassColors
    {
        public WordClassColors(WordClassType type, Color color, Color headColor)
        {
            GeneralWordClass = type;
            HeadColor = new SolidColorBrush(headColor);
            Color = new SolidColorBrush(color);
        }

        internal WordClassType GeneralWordClass { get; private set; }
        internal SolidColorBrush HeadColor { get; private set; }
        internal SolidColorBrush Color { get; private set; }
    }

    public class DecoratedSentenceViewModel : ViewModelBase, IResultPane
    {
        static Run Space = new Run(" ");

        public DecoratedSentenceViewModel()
        {
            View = new DecoratedSentenceView { Model = this };
            Options = null;
        }

        public FlowDocument Document
        {
            get
            {
                return document ?? (document = new FlowDocument
                {
                    FontFamily = new FontFamily("Palatino Linotype"),
                    FontSize = 18.0
                });
            }
            set
            {
                document = value;
            }
        }
        FlowDocument document;

        #region Colors

        IList<WordClassColors> ColorList
        {
            get
            {
                if (colorList == null)
                {
                    colorList = new WordClassColors[] 
                    {
                       new WordClassColors(WordClassType.Adjective, 
                                           Color.FromRgb(0x00, 0x5C, 0x00), 
                                           Color.FromRgb(0x00, 0xCC, 0x00)),
                       new WordClassColors(WordClassType.Adverb, 
                                           Color.FromRgb(0x74, 0x4A, 0x00), 
                                           Color.FromRgb(0xFF, 0xAA, 0x00)),
                       new WordClassColors(WordClassType.Conjunction, 
                                           Color.FromRgb(0x44, 0x44, 0x44), 
                                           Color.FromRgb(0x99, 0x99, 0x99)),
                       new WordClassColors(WordClassType.Interjection, 
                                           Color.FromRgb(0xff, 0x0d, 0xff), 
                                           Color.FromRgb(0xff, 0x0d, 0xff)),
                       new WordClassColors(WordClassType.Noun,
                                           Color.FromRgb(0x00, 0x21, 0x6C),
                                           Color.FromRgb(0x04, 0x4B, 0xED)),
                       new WordClassColors(WordClassType.Verb,
                                           Color.FromRgb(0x74, 0x00, 0x00), 
                                           Color.FromRgb(0xFF, 0, 0))
                    };
                }
                return colorList;
            }
        }
        IList<WordClassColors> colorList;
	
        Dictionary<WordClassType, WordClassColors> ColorMap
        {
            get
            {
                return colorMap ?? (colorMap = ColorList.ToDictionary(x => x.GeneralWordClass));
            }
        }
        Dictionary<WordClassType, WordClassColors> colorMap;

        public SolidColorBrush OverlapBrush
        {
            get
            {
                if (overlapBrush == null)
                {
                    overlapBrush = new SolidColorBrush(Color.FromRgb(0x74, 0x74, 0x00));
                }
                return overlapBrush;
            }
        }
        SolidColorBrush overlapBrush;

        #endregion

        #region Font Properties

        public FontFamily Monospace
        {
            get
            {
                if (monospace == null)
                {
                    monospace = new FontFamily("Consolas");
                }
                return monospace;
            }
        }
        FontFamily monospace;
	
        #endregion
        
        #region IResultPane Members

        public ToolBar Options { get; set; }

        public string Title
        {
            get
            {
                return title ?? (title = "Decorated Sentence");
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
            Document.Blocks.Clear();
        }

        public void OnSentence(Sentence s)
        {
            if (s.Length == 0)
                return;

            if( s.Words.All(w => string.IsNullOrWhiteSpace(w.Instance)))
                return;

            var p = new Paragraph();

            if (!s.GeneralSyntaxProperties.IsComplete)
                p.Typography.Capitals = FontCapitals.AllSmallCaps;

            ISentenceNode[] segments;
            ISentenceNode leadNode;
            ISentenceNode lastLead = null;
            int m = 1;
            Inline r;

            for (int i = 0; i < s.Length; i++)
            {
                segments = s[i].ToArray();
                leadNode = segments.First();

                // Space
                if (i != 0)
                {
                    r = new Run(" ");
                    p.Inlines.Add(r);
                }
                
                // Start this word
                r = new Run(s.Words[i].Instance);

                // start phrase marker logic
                if (leadNode != lastLead && leadNode.GetType() != typeof(WordInstance))
                {
                    if (leadNode.StartIndex == i)
                    {
                        var marker = new Run(m.ToString());
                        marker.Typography.Variants = FontVariants.Superscript;
                        p.Inlines.Add(marker);
                    }

                    m++;
                }

                // part of multiple
                if (segments.Length > 2)
                {
                    r.Foreground = OverlapBrush;
                }

                // Determine the color of this phrase
                else if (leadNode != null)
                {
                    WordClassColors colors;
                    if (ColorMap.TryGetValue(leadNode.GeneralWordClass, out colors))
                    {
                        if (leadNode.Head != null && leadNode.Head.StartIndex == i)
                        {
                            r.Foreground = colors.HeadColor;
                            r = new Bold(r);
                        }
                        else
                            r.Foreground = colors.Color;
                    }
                }

                // Add extra information
                var np = leadNode as NounPhrase;
                if (np != null && np.IsPrepositionalPhrase)
                    r.FontStyle = FontStyles.Oblique;

                var vp = leadNode as VerbPhrase;
                if (vp != null && vp.IsPassive)
                    r.FontStyle = FontStyles.Italic;
                if( vp != null && vp.IsFinite )
                    r = new Underline(r);

                p.Inlines.Add(r);

                lastLead = leadNode;
            }

            // Add the general syntax properties block
            var propertyBlock = new Span(new LineBreak());
            propertyBlock.Background = Brushes.Wheat;
            propertyBlock.FontSize = 12;
            propertyBlock.FontFamily = Monospace;

            r = new Run(string.Format("[{0}", s.GetAll<NounPhrase>().Count(x => !x.IsPrepositionalPhrase).ToString()));
            propertyBlock.Inlines.Add(r);

            r = new Run(string.Format(".{0}", s.GetAll<NounPhrase>().Count(x => x.IsPrepositionalPhrase).ToString()));
            r.FontStyle = FontStyles.Italic;
            propertyBlock.Inlines.Add(r);

            r = new Run(string.Format(" {0}", s.GetAll<VerbPhrase>().Count(x => !x.IsPassive).ToString()));
            propertyBlock.Inlines.Add(r);

            r = new Run(string.Format(".{0}", s.GetAll<VerbPhrase>().Count(x => x.IsPassive).ToString()));
            r.FontStyle = FontStyles.Italic;
            propertyBlock.Inlines.Add(r);

            r = new Run(string.Format(" {0}]", s.GeneralSyntaxProperties));
            propertyBlock.Inlines.Add(r);

            p.Inlines.Add(propertyBlock);

            Document.Blocks.Add(p);
        }

        public void OnEndAnalysis()
        {
        }

        public void OnCopy()
        {
            
        }

        #endregion
    }
}
