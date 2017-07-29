using System.Linq;
using Bailiwick.DAL;
using Bailiwick.Models;

namespace Bailiwick.Corpora
{
    public class BrownCorpus : CorpusBase
    {
        public ILookup<string, Frequency> AlternateOneGram
        {
            get
            {
                if (alternateOneGram == null)
                {
                    var type = GetType();
                    var assm = type.Assembly;
                    var name = type.Namespace + ".BrownFrequencyRaw.txt";
                    var loader = new DistributionFormatter();

                    using (var stream = assm.GetManifestResourceStream(name))
                    {
                        alternateOneGram = loader.Deserialize(stream).ToLookup(f => f.Normalized);
                    }
                }

                return alternateOneGram;
            }
        }
        ILookup<string, Frequency> alternateOneGram;
        
        protected override string CorpusResourceName
        {
            get { return GetType().Namespace + ".BrownFrequency.txt"; }
        }
    }
}
