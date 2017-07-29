using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.UI.Strategies
{
    public class KwicLemma : IKwicStrategy
    {
        #region IKwicStrategy Members

        public string Title
        {
            get { return "Lemma"; }
        }

        public IEnumerable<string> KeyWords(GlossDistribution distribution)
        {
            return distribution.Select(x => x.Key.Lemma).OrderBy(x => x);
        }

        public Func<WordInstance, bool> BuildFilter(string keyWord)
        {
            return x => x.Lemma == keyWord;
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }
    }
}
