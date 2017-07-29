using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.WordClassType>>;

namespace Bailiwick.Models
{
    public interface ISyntax
    {
        void Increment(IGloss previous, IGloss next);

        double PercentNextGivenPrevious(WordClassType previous, WordClassType next);
        SyntaxPartials PreviousPartials(WordClassType previous);
        double PercentPrevious(WordClassType previous);
        double PercentNext(WordClassType next);
    }
}
