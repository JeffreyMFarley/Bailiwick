using Bailiwick.Models;
using Bailiwick.Parsers;
using Bailiwick.UI;
using Esoteric.UI;
using Esoteric.BLL.Interfaces;
using Esoteric.BLL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bailiwick.Analysis
{
    public class SyntaxEngine
    {
        #region Constructor

        public SyntaxEngine(IProgressUI feedback, ISentenceOperation callback)
        {
            Debug.Assert(feedback != null);
            Debug.Assert(callback != null);

            Feedback = feedback;
            Callback = callback;
        }

        #endregion

        #region Properties

        public IProgressUI Feedback { get; private set; }

        public ISentenceOperation Callback { get; private set; }

        public bool Tokenize { get; set; }

        #endregion

        #region Public Methods

        public void Run(string[] morphemes)
        {
            Sentence.ResetIndex();

            Feedback.Beginning();

            var corpus = new Corpora.CocaCorpus();
            var tryConvert = new TryConvert(corpus);
            var forceConvert = new ForceConvert();

            var tokenizer = new ParsingEngine(corpus);
            var shallowParser = new Thoughts.Engine();
            var classifier = new Morphology.Classifier(corpus);
            var pipeline = new ISentenceOperation[] 
            {
                new PhraseDirector(tryConvert, forceConvert),
                new PostPhraseBuilder(tryConvert, forceConvert)
            };

            Feedback.SetMinAndMax(0, morphemes.Length);

            IEnumerable<string> source = morphemes;
            if (Tokenize) 
            {
                var tokenSegmenter = new WordSegmentation(corpus);

                source = tokenSegmenter.Segment(morphemes);
            }

            var tokens = source.SelectMany(x => tokenizer.Parse(x));
            var thoughts = shallowParser.Process(tokens);
            foreach (var sentence in classifier.Process(thoughts))
            {
                foreach (var op in pipeline)
                    op.Process(sentence);

                Callback.Process(sentence);

                Feedback.Increment();
            }

            Feedback.Finished();
        }
        
        #endregion

    }
}
