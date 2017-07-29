
namespace Bailiwick.Corpora
{
    public class ScaffoldingCorpus : CorpusBase
    {
        protected override string CorpusResourceName
        {
            get { return GetType().Namespace + ".Scaffold.txt"; }
        }
    }
}
