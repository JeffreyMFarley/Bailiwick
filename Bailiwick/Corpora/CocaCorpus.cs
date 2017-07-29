using System.Collections.Generic;
using System.Linq;
using Bailiwick.DAL;
using Bailiwick.Models;
using Esoteric.Collections;
using Esoteric.DAL;

namespace Bailiwick.Corpora
{
    public class CocaCorpus : CorpusBase
    {
        protected override string CorpusResourceName
        {
            get { return GetType().Namespace + ".COCA.txt"; }
        }

        protected override IEnumerable<Frequency> LoadAdditionalRecords()
        {
            // 's
            yield return new Frequency("'s", WordClasses.Specifics["VBZ"], "is", 2439692);

            // Determiner/Pronoun
            yield return new Frequency("those", WordClasses.Specifics["PN2"], "that", 434590);

            // Verb/Noun/Adjective
            yield return new Frequency("chuck", WordClasses.Specifics["NN1"], "chuck", 2280);
            yield return new Frequency("preserving", WordClasses.Specifics["VVG"], "preserve", 3802);
            yield return new Frequency("processing", WordClasses.Specifics["VVG"], "process", 11371);
        }
    }
}
