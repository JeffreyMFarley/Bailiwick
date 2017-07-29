using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models
{
    public class Thought
    {
        public Thought(WordInstance[] words)
        {
            Words = words;
        }

        public WordInstance[] Words { get; private set; }
        public bool IsInterrogative { get; set; }
    }
}
