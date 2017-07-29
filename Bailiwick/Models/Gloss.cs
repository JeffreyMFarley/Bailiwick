
namespace Bailiwick.Models
{
    public sealed class Gloss : IGloss
    {
        static public readonly Gloss EllipsisNoun = new Gloss("…", WordClasses.EllipsisNoun, "…");

        public Gloss(string normalized, WordClass partOfSpeech, string lemma = null)
        {
            Normalized = normalized;
            PartOfSpeech = partOfSpeech;
            Lemma = lemma ?? string.Empty;
        }

        public WordClass PartOfSpeech
        {
            get;
            private set;
        }

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

        public override int GetHashCode()
        {
            return Normalized.GetHashCode() ^ PartOfSpeech.GetHashCode();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Lemma))
                return string.Format("{0:G}", PartOfSpeech.General);
            else
                return string.Format("{0:G} - {1}", PartOfSpeech.General, Lemma);
        }
    }
}
