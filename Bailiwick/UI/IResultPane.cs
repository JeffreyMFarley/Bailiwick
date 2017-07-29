using Bailiwick.Models;
using System.Windows.Controls;
using Esoteric.UI;

namespace Bailiwick.UI
{
    public interface IResultPane : IViewModel
    {
        ToolBar Options { get; }
        string Title { get; }

        void OnStartAnalysis(string documentName);
        void OnSentence(Sentence s);
        void OnEndAnalysis();

        void OnCopy();
    }
}
