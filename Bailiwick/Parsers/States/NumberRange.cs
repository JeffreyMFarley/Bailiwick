using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Parsers.States
{
    internal class NumberRange : Number
    {
        public override IEnumerable<WordInstance> Extract(IContext c, string s)
        {
            yield return new WordInstance(s, WordClasses.Specifics["MCMC"]);
        }
    }
}
