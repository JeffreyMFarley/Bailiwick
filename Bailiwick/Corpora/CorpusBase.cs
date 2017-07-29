using System.Collections.Generic;
using System.Linq;
using Bailiwick.DAL;
using Bailiwick.Models;
using Esoteric.Collections;
using Esoteric.DAL;
using GlossPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.IGloss>>;
using SyntaxPartials = System.Collections.Generic.IList<Bailiwick.Models.Percentage<Bailiwick.Models.WordClassType>>;

namespace Bailiwick.Corpora
{
    abstract public class CorpusBase : ICorpus, IDistribution<string>
    {
        virtual protected string SyntaxResourceName
        {
            get { return GetType().Namespace + ".BrownSyntax.xml"; }
        }

        abstract protected string CorpusResourceName { get; }

        GlossPartials EmptyPartials
        {
            get
            {
                return emptyPartials ?? (emptyPartials = new List<Percentage<IGloss>>());
            }
        }
        GlossPartials emptyPartials;

        #region ICorpus Members

        public ISyntax Syntax
        {
            get
            {
                if (syntax == null)
                {
                    var er = new EmbeddedResourceRepository<Syntax>();
                    var name = SyntaxResourceName;
                    syntax = er.Load(GetType().Assembly, name);
                    syntax.OnLoaded();
                }
                return syntax;
            }
        }
        Syntax syntax;

        public ILookup<string, Frequency> OneGram
        {
            get
            {
                if (oneGram == null)
                {
                    var type = GetType();
                    var assm = type.Assembly;
                    var name = CorpusResourceName;
                    var loader = new DistributionFormatter();

                    using (var stream = assm.GetManifestResourceStream(name))
                    {
                        var basic = loader.Deserialize(stream);
                        oneGram = basic.Union(LoadAdditionalRecords()).ToLookup(f => f.Normalized);
                    }
                }

                return oneGram;
            }
        }
        ILookup<string, Frequency> oneGram;

        public IDictionary<string, string> Abbreviations
        {
            get
            {
                if (abbreviations == null)
                {
                    var type = GetType();
                    var assm = type.Assembly;
                    var name = type.Namespace + ".Abbreviations.txt";
                    var loader = new TabSeparatedFormatter();

                    using (var stream = assm.GetManifestResourceStream(name))
                    {
                        abbreviations = loader.Deserialize(stream).ToDictionary(f => f[0], f => f[1]);
                    }
                    /*
                    cc.	NN2
                     */
                }
                return abbreviations;
            }
        }
        IDictionary<string, string> abbreviations;

        public SyntaxPartials OpenClassRatios
        {
            get 
            {
                if (openClassRatios == null)
                {
                    var q = from a in OneGram.SelectMany(s => s)
                            where !a.PartOfSpeech.General.IsClosedWordClass() 
                                && a.PartOfSpeech.General != WordClassType.Number
                                && a.PartOfSpeech.General != WordClassType.Interjection
                            group a by a.PartOfSpeech.General into b
                            select new 
                            {
                                General = b.Key,
                                Count = b.Sum(x => x.Count)
                            };

                    var result = q.ToArray();
                    double sum = result.Sum(x => x.Count);

                    var q1 = from a in result
                             select new Percentage<WordClassType>(a.General, a.Count / sum);

                    openClassRatios = q1.ToArray();
                }

                return openClassRatios;
            }
        }
        SyntaxPartials openClassRatios;

        public GlossPartials Classifications(IGloss wordInstance)
        {
            if (wordInstance == null)
                return EmptyPartials;

            var q0 = OneGram[wordInstance.Normalized].ToArray();
            double sum = q0.Sum(x => x.Count);
            if (sum == 0.0)
                return EmptyPartials;

            var q1 = from a in q0
                     orderby a.Count descending
                     select new Percentage<IGloss>(a, a.Count / sum);

            return q1.ToArray();
        }

        #endregion

        #region IDistribution<string> Members

        public long TotalCount
        {
            get
            {
                if (totalCount == null)
                {
                    totalCount = OneGram.SelectMany(x => x).Sum(x => x.Count);
                }
                return totalCount.Value;
            }
        }
        long? totalCount;

        public void Increment(string value)
        {
            throw new System.NotImplementedException();
        }

        public void Decrement(string value)
        {
            throw new System.NotImplementedException();
        }

        public double Frequency(string value)
        {
            if (TotalCount == 0)
                return 0.0f;

            double numerator = OneGram[value].Sum(x => x.Count);
            return (numerator / ((double)TotalCount));
        }

        #endregion

        #region Virtual Methods

        virtual protected IEnumerable<Frequency> LoadAdditionalRecords() 
        {
            yield break;
        }

        #endregion

    }
}
