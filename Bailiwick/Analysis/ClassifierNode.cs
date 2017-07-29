using System;
using Bailiwick.Models;

namespace Bailiwick.Analysis
{
    public class ClassifierNode
    {
        public ClassifierNode(Percentage<WordInstance> head, ISyntax syntax, double denominator)
        {
            Instance = head.Value;
            Syntax = syntax;
            Score = CalculateScore(WordClassType.Start, head, denominator);
            Impossible = false;

            ScanBuilder = new SyntaxScanBuilder();
            ScanBuilder.Update(head.Value);
        }

        public ClassifierNode(ClassifierNode head, Percentage<WordInstance> tail, double denominator)
        {
            Root = head;
            Instance = tail.Value;
            Syntax = head.Syntax;
            Score = head.Score + CalculateScore(head.Instance.GeneralWordClass, tail, denominator);
            Impossible = head.Impossible;

            ScanBuilder = new SyntaxScanBuilder(head.ScanBuilder);
            ScanBuilder.Update(tail.Value);
        }

        public ClassifierNode(ClassifierNode node, WordInstance wordToAppend)
        {
            Root = node.Root;
            Instance = new WordInstance(node.Instance.Instance + wordToAppend.Instance, node.Instance);
            Syntax = node.Syntax;
            Score = node.Score;
            Impossible = node.Impossible;

            ScanBuilder = new SyntaxScanBuilder(node.ScanBuilder);
        }

        ISyntax Syntax { get; set; }
        public WordInstance Instance { get; private set; }
        public ClassifierNode Root { get; private set; }
        public double Score { get; private set; }
        public SyntaxScanBuilder ScanBuilder { get; private set; }
        public bool Impossible { get; set; }

        double CalculateScore(WordClassType previous, Percentage<WordInstance> next, double denominator)
        {
            if (denominator == 0.0)
                return double.NegativeInfinity;

            var pba = Syntax.PercentNextGivenPrevious(previous, next.Value.GeneralWordClass);
            var pa = next.Partial;
           
            return Math.Log10((pba * pa) / denominator);
        }
    }
}
