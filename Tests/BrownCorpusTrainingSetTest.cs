using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Bailiwick.Morphology;
using Bailiwick.Morphology.States;
using Bailiwick.Models;
using Bailiwick.Parsers;
using Bailiwick.Tests.Training;
using Esoteric.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bailiwick.Tests
{
    [Flags]
    internal enum TestOutput
    {
        Nothing = 0,
        Summary = 1,
        Details = 2,
        Grades = 4,
        ParseErrors = 8,
        ListKeys = 16,
        ListErrors = 32 + 8,
        CompareToBaseline = 64,

        All = 127,
        HighLevel = Summary | Grades | ParseErrors | ListKeys | CompareToBaseline,
        Scoping = Grades | CompareToBaseline
    }

    internal class Incorrect
    {
        internal string key;
        internal WordClass expected;
        internal WordClass actual;
        internal string normalized;
        internal string lemma;

        internal string Bin()
        {
            if (string.Compare(expected.Specific, actual.Specific) <= 0)
                return string.Format("{0}~{1}", expected.Specific, actual.Specific);
            return string.Format("{1}~{0}", expected.Specific, actual.Specific);
        }
    }

    [TestClass]
    public class BrownCorpusTrainingSetTest
    {
        #region Class Properties

        static Suite BrownCorpus;
        static ParsingEngine tokenizer;
        static Thoughts.Engine shallowParser;
        static ICorpus corpus;
        static Classifier classifier;
        static TraceListener traceListener;
        static TraceSource[] traceSources;
        static StringDistribution predicateTally;

        static Scores Baseline;

        #endregion
        #region Unit Test Framework Members

        public TestContext TestContext { get; set; }

        List<Incorrect> ParseErrors = new List<Incorrect>();
        Scores Scores = new Scores();

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            BrownCorpus = new Suite();

            Sentence.ResetIndex();
            corpus = new Corpora.CocaCorpus();
            tokenizer = new ParsingEngine(corpus);
            shallowParser = new Thoughts.Engine();
            classifier = new Classifier(corpus);
            predicateTally = Classifier.PredicateTally;

            traceListener = new TestContextTraceListener(context);
            traceSources = new TraceSource[] { TraceSources.Classifier, TraceSources.States };

            var baselineFileName = Path.Combine(context.TestRunDirectory, @"..\..\Tests\Training\Scores.txt");
            Baseline = Scores.Load(baselineFileName);

            foreach (var source in traceSources)
                source.Listeners.Add(traceListener);
        }

        [TestInitialize]
        public void Initialize()
        {
            ParseErrors.Clear();
            Scores.Clear();
            predicateTally.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var source in traceSources)
                source.Switch.Level = SourceLevels.Off;

            if (predicateTally.Count > 0)
            {
                Console.WriteLine("\n---- Predicate Tally -----");
                Console.WriteLine("\nPercentage\tCount\tC# Class\tMethod\tWord Class");
                foreach (var kvp in predicateTally.OrderByDescending(kvp => kvp.Value))
                {
                    var key = kvp.Key;
                    Console.WriteLine("{1:p}\t{2}\t{0}", key, predicateTally.Frequency(key), predicateTally[key]);
                }
            }
        }
        #endregion
        #region Fixture Setup Methods
        #endregion
        #region Common Routines

        void EnableTracing(SourceLevels level = SourceLevels.Information)
        {
            foreach (var source in traceSources)
                source.Switch.Level = level;
        }

        IEnumerable<Incorrect> Incorrect(string key, Sentence s, IEnumerable<TaggedWord> words)
        {
            var fn = new Func<WordInstance, Tuple<string, WordClass>, Incorrect>((x, y) =>
            {
                if (x.GeneralWordClass == y.Item2.General)
                    return null;

                return new Incorrect
                {
                    key = key,
                    actual = x.PartOfSpeech,
                    expected = y.Item2,
                    normalized = x.Normalized,
                    lemma = x.Lemma
                };
            });

            var zip = s.Words.Zip(words.SelectMany(x => x.Tags(),
                                  (word, tag) => new Tuple<string, WordClass>(word.Word, tag)),
                                  fn);
            return zip.Where(x => x != null);
        }

        void OutputSummary(string id, Sentence s, IEnumerable<TaggedWord> words, bool[] comparison, float score)
        {
            Console.Write(id); Console.Write('\t');
            Console.Write(Math.Round((decimal)score, 2)); Console.Write('\t');
            Console.Write(words.Count()); Console.Write("\t");
            Console.Write(comparison.Count()); Console.Write("\t");
            Console.Write(comparison.Count(x => !x)); Console.Write("\t");

            for (int i = 0; i < comparison.Length; i++)
            {
                if (comparison[i]) Console.Write('.');
                else Console.Write('x');
            }
            for (int i = comparison.Length; i < s.Words.Count; i++)
            {
                Console.Write('x');
            }
            Console.WriteLine();
        }

        void OutputDetail(string id, Sentence s, IEnumerable<TaggedWord> words, bool[] comparison, float score)
        {
            var expanded = words.SelectMany(x => x.Tags()).ToArray();
            var wordArray = words.Select(x => x.Word).ToArray();

            Console.Write(id); Console.Write("\t");
            Console.Write(Math.Round((decimal)score, 2)); Console.Write('\t');
            Console.Write(wordArray.Length); Console.Write("\t");
            Console.Write(comparison.Count()); Console.Write("\t");
            Console.WriteLine(comparison.Count(x => !x));

            Console.Write("P/F\t");
            Console.Write("i\t");
            Console.Write("word\t");
            Console.Write("baliwick\t");
            Console.WriteLine("brown");

            for (int i = 0; i < expanded.Length; i++)
            {
                if (i < comparison.Length)
                {
                    if (comparison[i]) Console.Write(".\t");
                    else Console.Write("x\t");
                    Console.Write(i); Console.Write('\t');
                    Console.Write(s.Words[i].Instance); Console.Write('\t');
                    Console.Write(s.Words[i].PartOfSpeech.Specific); Console.Write('\t');
                }
                else
                {
                    Console.Write("x\t");
                    Console.Write(i); Console.Write("\t\t\t");
                }
                Console.Write(expanded[i].Specific);
                if (i < wordArray.Length)
                {
                    Console.Write('\t'); Console.Write(wordArray[i]);
                }
                Console.WriteLine();
            }
        }

        float Execute(string id, TestOutput options = TestOutput.Details, float detailThreshhold = 99)
        {
            TraceSources.Classifier.TraceEvent(TraceEventType.Information, 0, id);

            float finalScore = 0;

            if (!id.Contains("?"))
            {
                var words = (from w in BrownCorpus.Sentences[id]
                             where w.Tag != "QD" && w.Tag != "QQ"
                             select w).ToArray();
                var tokens = words.SelectMany(x => tokenizer.Parse(x.Word));
                var thoughts = shallowParser.Process(tokens);

                var result = classifier.Process(thoughts).First();

                var a = words.SelectMany(x => x.Tags()).ToArray();
                var b = result.Words.Select(x => x.PartOfSpeech).ToArray();

                var comparison = b.Zip(a, (x, y) => x.General == y.General).ToArray();

                var distance = a.SequenceDistance(b, WordClass.GeneralComparer);

                var max = Convert.ToSingle(Math.Max(a.Count(), b.Count()));
                //                var fails = Convert.ToSingle(comparison.Count(x => !x));
                //                var passes = Convert.ToSingle(comparison.Count(x => x));
                var baseScore = Convert.ToSingle(comparison.Count() - distance) / max;


                finalScore = baseScore * 100;

                if (options.HasFlag(TestOutput.ParseErrors))
                    ParseErrors.AddRange(Incorrect(id, result, words));

                if (options.HasFlag(TestOutput.Summary))
                    OutputSummary(id, result, words, comparison, finalScore);

                if (finalScore < detailThreshhold && options.HasFlag(TestOutput.Details))
                    OutputDetail(id, result, words, comparison, finalScore);
            }
            else
                finalScore = -1;

            TraceSources.Classifier.TraceEvent(TraceEventType.Information, 0, "{0} {1}", Scores.Grade(finalScore), finalScore);

            return finalScore;
        }

        bool ExecuteList(IEnumerable<string> ids, TestOutput options = TestOutput.HighLevel, float detailThreshhold = 99)
        {
            foreach (var test in ids)
                Scores[test] = Execute(test, options, detailThreshhold);

            if (options.HasFlag(TestOutput.Grades))
                Scores.DumpGrades(options.HasFlag(TestOutput.ListKeys));

            if (options.HasFlag(TestOutput.ListErrors))
            {
                Console.WriteLine("\n---- All Errors -----");
                Console.WriteLine("\nNormalized\tLemma\tBin\tExpected\tActual\tKey");
                foreach (var e in ParseErrors)
                {
                    Console.Write(e.normalized); Console.Write('\t');
                    Console.Write(e.lemma); Console.Write('\t');
                    Console.Write(e.Bin()); Console.Write('\t');
                    Console.Write(e.expected.Specific); Console.Write('\t');
                    Console.Write(e.actual.Specific); Console.Write('\t');
                    Console.WriteLine(e.key);
                }
            }

            else if (options.HasFlag(TestOutput.ParseErrors))
            {
                Console.WriteLine("\n---- Parsing Errors -----");
                var gen0 = from a in ParseErrors
                           group a by a.Bin() into g
                           select g;

                foreach (var g0 in gen0.OrderByDescending(x => x.Count()).ThenBy(x => x.Key))
                {
                    var count = g0.Count();
                    Console.Write(g0.Key); Console.Write('\t');
                    Console.Write(count); Console.Write('\t');
                    if (options.HasFlag(TestOutput.ListKeys))
                        Console.WriteLine(string.Join(", ", g0.Take(20).Select(x => x.key)));
                    else
                        Console.WriteLine();
                }
            }

            if( options.HasFlag(TestOutput.CompareToBaseline) )
                Scores.Compare(Baseline);

            return Scores.AllPassed();
        }

        #endregion

        [TestMethod]
        public void ComparisonFormattingTest()
        {
            var thatFileName = Path.Combine(TestContext.TestRunDirectory, @"..\Scores.txt");
            var that = Scores.Load(thatFileName);
            that.Compare(Baseline);
        }

        [TestMethod]
        public void WholeCorpus_Test()
        {
            var passed = ExecuteList(BrownCorpus.Sentences.Select(x => x.Key), TestOutput.Grades | TestOutput.CompareToBaseline);

            var outFileName = Path.Combine(TestContext.TestRunDirectory, @"..\Scores.txt");
            Scores.Save(outFileName);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void A_Press_Reportage_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("A")
                           select a.Key;

            EnableTracing(SourceLevels.Error);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void B_Press_Editorial_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("B")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void C_Press_Reviews_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("C")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void D_Religion_Text_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("D")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void E_Skill_and_Hobbies_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("E")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void F_Popular_Lore_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("F")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void G_Belle_Lettres_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("G")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void H_Government_And_House_Organs_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("H")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void J_Learned_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("J")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void K_Fiction_General_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("K")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void L_Fiction_Mystery_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("L")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void M_Science_Fiction_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("M")
                           select a.Key;

            //EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void N_Fiction_Adventure_and_Western_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("N")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void P_Fiction_Romance_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("P")
                           select a.Key;

            EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void R_Humor_Test()
        {
            var filtered = from a in BrownCorpus.Sentences
                           where a.Key.StartsWith("R")
                           select a.Key;

            //EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(filtered, TestOutput.Scoping));
        }

        [TestMethod]
        public void ScrutinizeTheseTest()
        {
            var thatFileName = Path.Combine(TestContext.TestRunDirectory, @"..\Scores.txt");
            var that = Scores.Load(thatFileName);
            var tests = that.PerformedWorse(Baseline).Select(x => x.Item1).ToList();

            //EnableTracing(SourceLevels.Information);
            Assert.IsTrue(ExecuteList(tests, TestOutput.Details));
        }

        [TestMethod]
        public void ScrutinizeMissingVVGTest()
        {
            var tests = new string[] {
                "A02-048", "A04-079", "A06-027", "A07-028", "A09-021", "A09-040", "A10-032",
                "A13-028", "A13-059", "A16-034", "A22-011", "A22-042", "A23-025", "A24-086",
                "A25-094", "A26-095", "A27-079", "A30-023", "A35-059", "A37-018", "A40-017",
                "A40-076", "A42-085", "A43-111", "B01-049", "B07-073", "B09-030", "B09-091",
                "B10-011", "B10-061", "B12-051", "B15-023", "B16-024", "B19-017", "B19-098",
                "B22-027", "C01-027", "C06-078", "C07-038", "C09-027", "C12-039", "C15-098",
                "C16-024", "C16-092", "D02-030", "D02-050", "D03-013"
            };

            EnableTracing(SourceLevels.Warning);
            Assert.IsTrue(ExecuteList(tests));
        }

        [TestMethod]
        public void SimpleSentencesTest()
        {
            var tests = new string[] { 
                "A34-085"
                ,"B01-001","B07-089","B10-095","B20-031","B23-108",
                "C03-066","E12-030","E12-066","E17-081","E19-081","F13-078","F13-121","F24-024",
                "H02-029","H02-069","H02-071","H02-076","H02-086","H02-111","H15-094","J45-078",
                "J45-094","M02-101","P03-046"
            };

            EnableTracing(SourceLevels.Warning);
            Assert.IsTrue(ExecuteList(tests));
        }

        [TestMethod]
        public void VerboseDebuggingTest()
        {
            EnableTracing(SourceLevels.Verbose);
            Assert.IsTrue(Execute("D07-028") >= 80.0);
        }
    }

    public class TestContextTraceListener : TraceListener
    {
        private readonly TestContext _context;
        private readonly StringBuilder _sb = new StringBuilder();

        public TestContextTraceListener(TestContext context)
        {
            _context = context;
        }

        public override void Write(string message)
        {
            _sb.Append(message);
        }

        public override void WriteLine(string message)
        {
            _sb.Append(message);
            _context.WriteLine(_sb.ToString());
            _sb.Clear();
        }
    }
}
