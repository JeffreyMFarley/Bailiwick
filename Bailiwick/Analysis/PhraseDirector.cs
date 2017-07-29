using Bailiwick.Analysis.PhraseBuilders;
using Bailiwick.Models;

namespace Bailiwick.Analysis
{
    public class PhraseDirector : ISentenceOperation
    {
        public PhraseDirector(IConvert tryConvert, IConvert forceConvert)
        {
            TryConvert = tryConvert;
            ForceConvert = forceConvert;
        }

        public IConvert TryConvert { get; private set; }
        public IConvert ForceConvert { get; private set; }

        #region Builders

        IPhraseBuilder NBuilder
        {
            get
            {
                if (nBuilder == null)
                {
                    nBuilder = new PhraseBuilders.Nouns.StateMachineBuilder(TryConvert, ForceConvert, VBuilder);
                }
                return nBuilder;
            }
        }
        IPhraseBuilder nBuilder;

        IPhraseBuilder VBuilder
        {
            get
            {
                if (vBuilder == null)
                {
                    vBuilder = new VerbPhraseBuilder();
                }
                return vBuilder;
            }
        }
        IPhraseBuilder vBuilder;

        #endregion

        public void Process(Sentence s)
        {
            NBuilder.Reset();
            VBuilder.Reset();

            for (int i = 0; i < s.Length; i++)
            {
                var w = s.Words[i];

                if (w.GeneralWordClass == WordClassType.Not && VBuilder.State == PhraseBuilderStatus.Building)
                {
                    VBuilder.Process(w);
                    NBuilder.Finished();
                }
                else
                {
                    NBuilder.Process(w);
                }

                if (NBuilder.State == PhraseBuilderStatus.Done)
                {
                    ExtractNounPhrases(s);
                }

                if (VBuilder.State == PhraseBuilderStatus.Done)
                {
                    ExtractVerbPhrases(s);
                }
            }

            NBuilder.Finished();
            VBuilder.Finished();

            ExtractNounPhrases(s);
            ExtractVerbPhrases(s);
        }

        void ExtractNounPhrases(Sentence s)
        {
            s.AddRange(NBuilder.Phrases);
            NBuilder.Reset();
        }

        void ExtractVerbPhrases(Sentence s)
        {
            s.AddRange(VBuilder.Phrases);
            VBuilder.Reset();
        }
    }
}
