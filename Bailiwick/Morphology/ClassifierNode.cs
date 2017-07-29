using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bailiwick.Models;

namespace Bailiwick.Morphology
{
    public class ClassifierNode : IContext
    {
        static IContext NullContext
        {
            get
            {
                if (nullContext == null)
                {
                    var nullWord = new WordInstance("", WordClasses.BeforeClauseMarker);
                    nullContext = new ClassifierNode(nullWord, 1.0);
                }
                return nullContext;
            }
        }
        static IContext nullContext;

        public ClassifierNode(WordInstance word, double percentage)
        {
            Word = word;
            State = StateFactory.GetState(word);
            Percentage = percentage;
            Score = 0;
        }

        public ClassifierNode(ClassifierNode head, WordInstance word, double percentage)
        {
            Root = head;
            Word = word;
            State = StateFactory.GetState(word);
            Percentage = percentage;
            Score = head.Score + Math.Log10(percentage * head.Percentage);
        }

        public ClassifierNode(ClassifierNode node, string wordToAppend)
        {
            Root = node.Root;
            Word = new WordInstance(node.Word.Instance + wordToAppend, node.Word);
            State = StateFactory.GetState(Word);
            Percentage = node.Percentage;
            Score = node.Score;
        }

        public ClassifierNode Root { get; private set; }

        public double Score { get; private set; }

        #region IContext Members

        public IState State
        {
            get;
            private set;
        }

        public WordInstance Word
        {
            get;
            private set;
        }

        public double Percentage 
        { 
            get; 
            private set; 
        }

        public IContext Previous
        {
            get
            {
                return (Root != null) ? Root : NullContext;
            }
        }

        public bool AcceptAsNext(WordInstance w, double percentage)
        {
            return State.AcceptAsNext(w, percentage, this);
        }

        #endregion
    }
}
