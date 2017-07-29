using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers
{
    public interface IParsingState
    {
        void OnEntry(IContext c);
        void OnExit(IContext c);

        // Events
        IParsingState OnControl(IContext c);
        IParsingState OnDigit(IContext c);
        IParsingState OnLetter(IContext c);
        IParsingState OnPunctuation(IContext c);
        IParsingState OnSymbol(IContext c);
        IParsingState OnDone(IContext c);

        // Queries

        // Methods
        IEnumerable<WordInstance> Extract(IContext c, string s);
    }
}
