using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Morphology
{
    public interface IContext
    {
        IState State { get; }
        WordInstance Word { get; }
        double Percentage { get; }

        IContext Previous { get; }

        bool AcceptAsNext(WordInstance w, double percentage);
    }
}
