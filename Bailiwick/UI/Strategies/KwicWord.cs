using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.UI.Strategies
{
    public class KwicWord : IKwicStrategy
    {
        #region IKwicStrategy Members

        public string Title
        {
            get { return "Word"; }
        }

        public IEnumerable<string> KeyWords(GlossDistribution distribution)
        {
            return distribution.Select(x => x.Key.Normalized).OrderBy(x => x);
        }

        public Func<WordInstance, bool> BuildFilter(string keyWord)
        {
            return x => x.Normalized == keyWord;
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }
    }
}
