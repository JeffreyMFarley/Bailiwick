using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models
{
    public class ProcessVerb
    {
        public ProcessVerb(string verb, string taxonomy, int level)
        {
            Verb = verb;
            Taxonomy = taxonomy;
            Level = level;
        }

        public string Verb { get; private set; }
        public string Taxonomy { get; private set; }
        public int Level { get; private set; }
    }
}
