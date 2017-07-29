using System;

namespace Bailiwick.Models
{
    public class Frequency : IGloss
    {
        public Frequency(IGloss source)
        {
            Normalized = source.Normalized;
            PartOfSpeech = source.PartOfSpeech;
            Lemma = source.Lemma;
        }

        public Frequency(string normalized, WordClass partOfSpeech, string lemma = null, Int64 count = 0)
        {
            Normalized = normalized;
            PartOfSpeech = partOfSpeech;
            Lemma = lemma;
            Count = count;
        }

        public Int64 Count { get; set; }

        public string Lemma
        {
            get;
            private set;
        }

        public string Normalized
        {
            get;
            private set;
        }

        public WordClass PartOfSpeech
        {
            get;
            private set;
        }
    }
}
