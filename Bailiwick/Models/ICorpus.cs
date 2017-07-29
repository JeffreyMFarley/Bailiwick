using System.Collections.Generic;
using System.Linq;
using GlossPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.IGloss>>;
using SyntaxPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.WordClassType>>;

namespace Bailiwick.Models
{
    public interface ICorpus
    {
        ISyntax Syntax { get; }
        ILookup<string, Frequency>  OneGram { get; }
        IDictionary<string, string> Abbreviations { get; }
        SyntaxPartials OpenClassRatios { get; }

        GlossPartials Classifications(IGloss wordInstance);
    }
}
