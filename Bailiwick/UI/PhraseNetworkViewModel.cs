using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Esoteric.UI;
using Bailiwick.Models;
using QuickGraph;
using QuickGraph.Collections;

namespace Bailiwick.UI
{
    public class PhraseEdge : IEdge<Colocates>
    {
        public Colocates Source { get; set; }
        public Colocates Target { get; set; }
    }

    public class PhraseNetworkViewModel : ViewModelBase, IResultPane
    {
        public PhraseNetworkViewModel()
        {
            View = new PhraseNetworkView { Model = this };
            Options = null;
        }

        BidirectionalGraph<Colocates, PhraseEdge> Graph
        {
            get
            {
                return graph ?? (graph = new BidirectionalGraph<Colocates, PhraseEdge>());
            }
            set
            {
                graph = value;
            }
        }
        BidirectionalGraph<Colocates, PhraseEdge> graph;

        #region Binding Properties

        public ObservableCollection<Colocates> Vertexes
        {
            get
            {
                return vertexes ?? (vertexes = new ObservableCollection<Colocates>());
            }
        }
        ObservableCollection<Colocates> vertexes;

        public Colocates SelectedVertex
        {
            get
            {
                return selectedVertex;
            }
            set
            {
                selectedVertex = value;
                OnPropertyChanged("SelectedVertex");
                OnPropertyChanged("Incoming");
                OnPropertyChanged("Outgoing");
            }
        }
        Colocates selectedVertex;

        public IEnumerable<Colocates> Incoming
        {
            get
            {
                if (SelectedVertex == null)
                {
                    yield break;
                }

                foreach (var s in Graph.InEdges(SelectedVertex).Select(e => e.Source))
                    yield return s;
            }
        }

        public IEnumerable<Colocates> Outgoing
        {
            get
            {
                if (SelectedVertex == null)
                {
                    yield break;
                }

                foreach (var s in Graph.OutEdges(SelectedVertex).Select(e => e.Target))
                    yield return s;
            }
        }

        #endregion

        #region IResultPane Members

        public ToolBar Options { get; private set; }

        public string Title
        {
            get
            {
                return title ?? (title = "Phrase Network");
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
            SelectedVertex = null;
            Vertexes.Clear();
            Graph.Clear();
        }

        public void OnSentence(Sentence s)
        {
            var e = from p in s.GetAll<IPhrase>()
                    where p.ClassColocates.Count > 0
                    select new Colocates(p.ClassColocates, p.GeneralWordClass, p.Head);

            var a = e.ToArray();
            if (a.Length <= 1)
                return;

            if (!Graph.ContainsVertex(a[0]))
                Graph.AddVertex(a[0]);

            for (int i = 1; i < a.Length; i++)
            {
                if (!Graph.ContainsVertex(a[i]))
                    Graph.AddVertex(a[i]);

                Graph.AddEdge(new PhraseEdge { Source = a[i - 1], Target= a[i] });
            }
        }

        public void OnEndAnalysis()
        {
            foreach (var v in Graph.Vertices.OrderBy(c => c.ToString()))
                Vertexes.Add(v);

            SelectedVertex = Vertexes.FirstOrDefault();
        }

        public void OnCopy()
        {
        }

        #endregion
    }
}
