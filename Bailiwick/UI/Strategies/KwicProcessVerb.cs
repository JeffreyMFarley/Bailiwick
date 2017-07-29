using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Analysis;
using Bailiwick.DAL;
using Bailiwick.Models;

namespace Bailiwick.UI.Strategies
{
    public class KwicProcessVerb : IKwicStrategy
    {
        #region Word Analyzers

        ILookup<string, ProcessVerb> ProcessVerbs
        {
            get
            {
                if (processVerbs == null)
                {
                    var type = typeof(SyntaxEngine);
                    var assm = type.Assembly;
                    var name = type.Namespace + ".ProcessVerbs.txt";
                    var loader = new ProcessVerbFormatter();

                    using (var stream = assm.GetManifestResourceStream(name))
                    {
                        processVerbs = loader.Deserialize(stream).ToLookup(f => f.Verb);
                    }
                }
                return processVerbs;
            }
        }
        ILookup<string, ProcessVerb> processVerbs;

        #endregion
        #region IKwicStrategy Members

        public string Title
        {
            get { return "Process Verb"; }
        }

        public IEnumerable<string> KeyWords(GlossDistribution distribution)
        {
            var j = from a in distribution.Select(x => x.Key.Lemma)
                    join b in ProcessVerbs on a equals b.Key
                    select b;

            return j.SelectMany(x => x)
                .OrderBy(x => x.Level)
                .Select(x => x.Taxonomy);
        }

        public Func<WordInstance, bool> BuildFilter(string keyWord)
        {
            return x =>
            {
                if (ProcessVerbs.Contains(x.Lemma))
                    return ProcessVerbs[x.Lemma].Any(a => a.Taxonomy == keyWord);

                return false;
            };
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }
    }
}
