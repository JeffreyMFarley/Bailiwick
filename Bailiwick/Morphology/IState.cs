using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Morphology
{
    public interface IState
    {
        bool AcceptAsNext(WordInstance w, double percentage, IContext c);
    }
}
