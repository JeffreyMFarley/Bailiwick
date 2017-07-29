using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;

namespace Bailiwick.UI.Strategies
{
    public class KwicWordClassType : IKwicStrategy
    {
        WordClassType _filteringType = WordClassType.Unclassified;

        public KwicWordClassType(WordClassType filteringType)
        {
            _filteringType = filteringType;
        }

        #region IKwicStrategy Members

        public string Title
        {
            get { return string.Format("{0:G}", _filteringType); }
        }

        public IEnumerable<string> KeyWords(GlossDistribution distribution)
        {
            return from k in distribution.Keys
                   where k.PartOfSpeech.General == _filteringType
                   orderby k.Lemma
                   select k.Lemma;
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
