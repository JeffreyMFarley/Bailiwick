using System.Diagnostics;

namespace Bailiwick.Models
{
    public class WordInstance : IGloss, ISentenceNode
    {
        #region Factory Methods
        static public WordInstance CreateWithGloss(string instance, IGloss gloss)
        {
            return new WordInstance(instance, gloss);
        }

        static public WordInstance CreateWithWordClass(string instance, WordClass wc)
        {
            return new WordInstance(instance, wc);
        }
        #endregion

        #region Constructors
        public WordInstance() { }
        public WordInstance(string instance)
        {
            Instance = instance ?? string.Empty;
            Lemma = string.Empty;
            Normalized = Instance.ToLower();
            PartOfSpeech = WordClasses.Unclassified;
        }

        public WordInstance(string instance, string normalized)
        {
            Instance = instance ?? string.Empty;
            Lemma = string.Empty;
            Normalized = normalized ?? Instance.ToLower();
            PartOfSpeech = WordClasses.Unclassified;
        }

        public WordInstance(string instance, WordClass partOfSpeech)
        {
            Instance = instance ?? string.Empty;
            Lemma = string.Empty;
            Normalized = Instance.ToLower();
            PartOfSpeech = partOfSpeech;
        }

        public WordInstance(string instance, IGloss source)
        {
            Debug.Assert(source != null);
            
            Instance = instance;
            Lemma = source.Lemma;
            Normalized = source.Normalized;
            PartOfSpeech = source.PartOfSpeech;
        }
        #endregion

        public string Instance
        {
            get;
            private set;
        }

        #region IGloss Members

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
            set;  // TODO: Make immutable
        }

        #endregion

        #region ISentenceNode Members

        public WordClassType GeneralWordClass
        {
            get { return PartOfSpeech.General; }
        }

        public WordInstance Head
        {
            get { return this; }
        }

        public int StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; }
        }
        int startIndex = -1;

        public int EndIndex { get { return StartIndex; } }

        #endregion

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Instance, PartOfSpeech.ToString());
        }
    }
}
