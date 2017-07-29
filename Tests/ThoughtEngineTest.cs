using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Bailiwick.Thoughts;
using Bailiwick.Models;
using Bailiwick.Parsers;
using Bailiwick.Tests.Training;
using Esoteric.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bailiwick.Tests
{
    [TestClass()]
    public class ThoughtEngineTest
    {
        #region Class Properties

        static Suite BrownCorpus;
        static ParsingEngine parser;
        static ICorpus corpus;
        static StringDistribution predicateTally;

        #endregion
        #region Unit Test Framework Members
        
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            BrownCorpus = new Suite();

            Sentence.ResetIndex();
            corpus = new Corpora.CocaCorpus();
            parser = new ParsingEngine(corpus);
            predicateTally = Thoughts.Engine.PredicateTally;
        }

        [TestInitialize]
        public void Initialize()
        {
            predicateTally.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var key in predicateTally.Keys)
                Console.WriteLine("{1:p}\t{2}\t'{0}'", key, predicateTally.Frequency(key), predicateTally[key]);
        }

        #endregion
        #region Fixture Setup Methods

        static internal Engine BuildTarget()
        {
            var result = new Engine();
            return result;
        }

        #endregion
        #region Basic Engine Tests

        [TestMethod]
        public void NormalWritingTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "The cat and dog are in the house. The house is in the city.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(9, actual[0].Words.Length);
            Assert.AreEqual(7, actual[1].Words.Length);
        }

        [TestMethod]
        public void CRLFTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "A list of the following:\nAccelerate\r\nBehold\rCollated\n\rDefined";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(5, actual.Length);
            Assert.AreEqual(6, actual[0].Words.Length);
            Assert.AreEqual(2, actual[1].Words.Length);
            Assert.AreEqual(2, actual[2].Words.Length);
            Assert.AreEqual(2, actual[3].Words.Length);
            Assert.AreEqual(1, actual[4].Words.Length);
        }

        [TestMethod]
        public void ExclamationTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "That's so great!! I'm glad it happened!";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(5, actual[0].Words.Length);
            Assert.AreEqual(6, actual[1].Words.Length);
        }

        [TestMethod]
        public void InterrogativeTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "Where are you?? I can't see where you are.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(4, actual[0].Words.Length);
            Assert.IsTrue(actual[0].IsInterrogative);
            Assert.AreEqual(8, actual[1].Words.Length);
            Assert.IsFalse(actual[1].IsInterrogative);
        }

        [TestMethod]
        public void ProcessTrickyTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "Mr. A. J. Miller, a Sen. from Mass., responded to Rev. K. D. Donaldson of St. Paul, Ore. in a letter describing how he should recuse himself from the comittee meeting. In response, the Rev. declined.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(35, actual[0].Words.Length);
            Assert.AreEqual(7, actual[1].Words.Length);
        }

        [TestMethod]
        public void AbbreviationNewlineTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "Paris, Texas sp.\n- The Board of Education decided that";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(5, actual[0].Words.Length);
            Assert.AreEqual(7, actual[1].Words.Length);
        }

        [TestMethod]
        public void AbbreviationExclamationTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "It was Mel Sp.! He saw the fish.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(5, actual[0].Words.Length);
            Assert.AreEqual(5, actual[1].Words.Length);
        }

        [TestMethod]
        public void AbbreviationQuestionTest()
        {
            // Setup
            var target = BuildTarget();
            var text = "Was it Mel Sp.? What did he see?";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(5, actual[0].Words.Length);
            Assert.AreEqual(5, actual[1].Words.Length);
        }

        [TestMethod]
        public void AbbreviationPeriod1Test()
        {
            // Setup
            var target = BuildTarget();
            var text = "He was from Chicago, Ill.. He was used to the cold.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(7, actual[0].Words.Length);
            Assert.AreEqual(7, actual[1].Words.Length);
        }

        [TestMethod]
        public void AbbreviationPeriod2Test()
        {
            // Setup
            var target = BuildTarget();
            var text = "He was from Chicago, Ill. . He was used to the cold.";
            var stream = text.Split(new char[] { ' ' });
            var tokens = stream.SelectMany(x => parser.Parse(x));

            // Execute
            var actual = target.Process(tokens).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(7, actual[0].Words.Length);
            Assert.AreEqual(7, actual[1].Words.Length);
        }

        #endregion
        #region Brown Corpus Test Helpers

        [Flags]
        enum BrownTestResult
        {
            Passed = 0,
            IsNull = 1,
            TooManyThoughts = 2,
            TooFewThoughts = 4,
            ThoughtTooShort = 8,
            NextThoughtTooShort = 16,
            BadInterrogative = 32,
            ThrewException = 64
        }

        string[] droppedPunctuation = { "QD", "QQ" };

        void OutputResult(string id, BrownTestResult result, string id2="")
        {
            if (result == BrownTestResult.Passed)
                return;

            StringBuilder sb = new StringBuilder(id);
            if (!string.IsNullOrEmpty(id2))
                sb.AppendFormat("~{0}", id2);
            if (result.HasFlag(BrownTestResult.IsNull))
                sb.Append(" Is Null");
            if (result.HasFlag(BrownTestResult.TooManyThoughts))
                sb.Append(" Too Many Thoughts");
            if (result.HasFlag(BrownTestResult.TooFewThoughts))
                sb.Append(" Not Enough Thoughts");
            if (result.HasFlag(BrownTestResult.ThoughtTooShort))
                sb.Append(" Short of Words");
            if (result.HasFlag(BrownTestResult.NextThoughtTooShort))
                sb.Append(" Second thought short of Words");
            if (result.HasFlag(BrownTestResult.BadInterrogative))
                sb.Append(" Incorrect tagging of '?'");
            if (result.HasFlag(BrownTestResult.ThrewException))
                sb.Append(" Threw Exception");

            TestContext.WriteLine(sb.ToString());
        }

        IEnumerable<string> BrownWords(string id) 
        {
            return from w in BrownCorpus.Sentences[id]
                   where !droppedPunctuation.Contains(w.Tag)
                   select w.Word;
        }

        BrownTestResult SingleBrownTest(string id)
        {
            // Setup
            var target = BuildTarget();
            var words = BrownWords(id).ToArray();
            var tokens = words.SelectMany(x => parser.Parse(x));

            // Execute
            Thought[] actual = null;
            try
            {
                actual = target.Process(tokens).ToArray();
            }
            catch {
                return BrownTestResult.ThrewException;
            }

            // Assert
            BrownTestResult result = (actual == null || actual.Length == 0) ? BrownTestResult.IsNull : 0;
            if( result != BrownTestResult.Passed )
                return result;

            result |= (actual.Length != 1) ? BrownTestResult.TooManyThoughts : 0;
            result |= (actual[0].Words.Length < words.Length) ? BrownTestResult.ThoughtTooShort : 0;
            result |= (actual[0].IsInterrogative != id.Contains('?')) ? BrownTestResult.BadInterrogative : 0;

            return result;
        }

        IEnumerable<string> ChainSentences(IEnumerable<string> a0, IEnumerable<string> a1)
        {
            string s = string.Empty;
            foreach (var a in a0)
            {
                yield return a;
                s = a;
            }

            if( s != "." && s != "!" && s != "?" )
                yield return "\r\n";

            foreach (var a in a1)
                yield return a;
        }

        BrownTestResult PairBrownTest(string id0, string id1)
        {
            // Setup
            var target = BuildTarget();
            var words0 = BrownWords(id0).ToArray();
            var words1 = BrownWords(id1).ToArray();
            var tokens = ChainSentences(words0, words1).SelectMany(x => parser.Parse(x));

            // Execute
            Thought[] actual = null;
            try
            {
                actual = target.Process(tokens).ToArray();
            }
            catch
            {
                return BrownTestResult.ThrewException;
            }

            // Assert
            BrownTestResult result = (actual == null || actual.Length == 0) ? BrownTestResult.IsNull : 0;
            if (result != BrownTestResult.Passed)
                return result;

            result |= (actual.Length < 2) ? BrownTestResult.TooFewThoughts : 0;
            result |= (actual.Length > 2) ? BrownTestResult.TooManyThoughts : 0;
            result |= (actual[0].Words.Length < words0.Length) ? BrownTestResult.ThoughtTooShort : 0;
            result |= (actual.Length > 1 && actual[1].Words.Length < words1.Length) ? BrownTestResult.NextThoughtTooShort : 0;

            return result;
        }

        #endregion
        #region Brown Corpus Tests

        [TestMethod]
        public void WholeCorpus_IndividualSentences_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           select a.Key;

            DiscreteDistribution<BrownTestResult> tally = new DiscreteDistribution<BrownTestResult>();

            BrownTestResult overall = 0;
            foreach (var id in filtered)
            {
                var result = SingleBrownTest(id);
                tally.Increment(result);
                OutputResult(id, result);
                overall |= result;
            }

            foreach (var key in tally.Keys)
                TestContext.WriteLine("{1:p}\t{0}", key, tally.Frequency(key));

            Assert.IsTrue(overall == BrownTestResult.Passed);
        }

        [TestMethod]
        public void WholeCorpus_PairedSentences_Test()
        {
            var groups = from a in BrownCorpus.Sentences
//                         where a.Key.StartsWith("A")
                         group a by a.Key.Substring(0,3) into g
                         select g;

            DiscreteDistribution<BrownTestResult> tally = new DiscreteDistribution<BrownTestResult>();
            BrownTestResult overall = 0;

            foreach (var g in groups)
            {
                var len = g.Count() - 1;
                var a0 = g.Take(len).Select(x => x.Key).ToArray();
                var a1 = g.Skip(1).Take(len).Select(x => x.Key).ToArray();
                
                for (int i = 0; i < len; i++)
                {
                    var result = PairBrownTest(a0[i], a1[i]);
                    tally.Increment(result);
                    OutputResult(a0[i], result, a1[i]);
                    overall |= result;
                }
            }

            foreach (var key in tally.Keys)
                TestContext.WriteLine("{1:p}\t{0}", key, tally.Frequency(key));

            Assert.IsTrue(overall == BrownTestResult.Passed);
        }

        [TestMethod]
        public void DebugIndividualTest()
        {
            string[] ids = { 
                               "A10-099","A16-034","A16-064","A17-014","A18-097","A23-031","A24-096","A24-099","A26-098",
                               "B11-031","B24-024",
                               "C15-078","C15-081",
                               "E05-014",
                               "F29-058",
                               "G52-029",
                               "G64-016","G64-017","G68-061","G74-052?",
                               "H17-029",
                               "J15-023","J33-019?","J43-032","J73-145?","J77-055"
                           };

            BrownTestResult overall = 0;
            foreach (var id in ids)
            {
                var result = SingleBrownTest(id);
                OutputResult(id, result);
                overall |= result;
            }

            Assert.IsTrue(overall == BrownTestResult.Passed);
        }

        [TestMethod]
        public void DebugPairTest()
        {
            BrownTestResult overall = 0;

            string[] a0 = {
                              "A23-081","A31-045",
                              "B03-039","B10-083","B11-034",
                              "C09-051","C15-123","D01-053",
                              "D01-056","D16-052","D16-056",
                              "E07-053","E07-118","E27-094","E34-053","E35-033","E35-072",
                              "G64-017","G74-079","G74-081",
                              "H01-134","H02-026","H02-068","H07-014","H07-023","H07-025","H10-004","H10-009","H13-066",
                              "H14-037","H14-040","H14-055","H14-058","H14-061","H14-064","H14-070","H14-093","H14-100","H14-103",
                              "H17-075","H20-013","H22-065","H22-067","H22-069","H22-071","H24-095","H24-103",
                              "J01-066","J03-035","J04-003","J04-057","J04-067","J04-100","J05-070","J09-060",
                              "J14-002","J14-022","J14-029","J14-045","J14-064","J14-076","J14-105",
                              "J18-012","J18-023","J18-041","J18-043","J18-047","J18-048","J18-054","J18-057","J18-061","J18-067","J18-070","J18-073","J18-075","J18-078","J18-080","J18-097","J18-104","J18-109",
                              "J19-113",
                              "J20-002","J20-007","J20-010","J20-012","J20-015","J20-018","J20-032","J20-045","J20-047","J20-051","J20-066","J20-072","J20-077","J20-090","J20-094","J20-095","J20-104","J20-107",
                              "J21-007","J21-010","J21-012","J21-024","J21-027","J21-028","J21-035","J21-040","J21-046","J21-054","J21-058",
                              "J41-016","J41-022","J41-030","J41-057","J41-063","J45-001","J45-018","J45-035","J45-049","J45-050","J45-077","J45-087","J45-093",
                              "J51-006","J51-012","J51-020","J51-030","J51-057","J51-069",
                              "J69-086","J70-085","J72-010","J72-046","J73-056","J74-095","J75-037","J75-046","J76-069","J77-001","J77-053","J77-054","J77-055","J77-120","J79-029","J79-047","J79-048","J79-053","J79-074","J79-096","J80-005",
                              "P20-004"
                          };
            string[] a1 = {
                              "A23-082","A31-046",
                              "B03-040","B10-084","B11-035",
                              "C09-052","C15-124",
                              "D01-054","D01-057","D16-053","D16-057",
                              "E07-054","E07-119","E27-095","E34-054","E35-034","E35-073",
                              "G64-018","G74-080","G74-082",
                              "H01-135","H02-027","H02-069","H07-015","H07-024","H07-026","H10-005","H10-010","H13-067",
                              "H14-038","H14-041","H14-056","H14-059","H14-062","H14-065","H14-071","H14-094","H14-101","H14-104",
                              "H17-076","H20-014","H22-066","H22-068","H22-070","H22-072","H24-096","H24-104",
                              "J01-067","J03-036","J04-004","J04-058","J04-068","J04-101","J05-071","J09-061",
                              "J14-003","J14-023","J14-030","J14-046","J14-065","J14-077","J14-106",
                              "J18-013","J18-024","J18-042","J18-044","J18-048","J18-049","J18-055","J18-058","J18-062","J18-068","J18-071","J18-074","J18-076","J18-079","J18-081","J18-098","J18-105","J18-110",
                              "J19-114",
                              "J20-003","J20-008","J20-011","J20-013","J20-016","J20-019","J20-033","J20-046","J20-048","J20-052","J20-067","J20-073","J20-078","J20-091","J20-095","J20-096","J20-105","J20-108",
                              "J21-008","J21-011","J21-013","J21-025","J21-028","J21-029","J21-036","J21-041","J21-047","J21-055","J21-059",
                              "J41-017","J41-023","J41-031","J41-058","J41-064","J45-002","J45-019","J45-036","J45-050","J45-051","J45-078","J45-088","J45-094",
                              "J51-007","J51-013","J51-021","J51-031","J51-058","J51-070",
                              "J69-087","J70-086","J72-011","J72-047","J73-057","J74-096","J75-038","J75-047","J76-070","J77-002","J77-054","J77-055","J77-056","J77-121","J79-030","J79-048","J79-049","J79-054","J79-075","J79-097","J80-006",
                              "P20-005"
                          };

            // A - All false on LowerDotLower I'm guessing stripped punctuation is removing the context
            // B - false Accumulates on LowerDotLower, LetterDot
            // J18, J20 - All false Accumulates on LetterDot

            for (int i = 0; i < a0.Length; i++)
            {
                if( !a0[i].StartsWith("J") )
                    continue;

                var result = PairBrownTest(a0[i], a1[i]);
                OutputResult(a0[i], result, a1[i]);
                overall |= result;
            }

            Assert.IsTrue(overall == BrownTestResult.Passed);
        }

        #endregion
    }
}
