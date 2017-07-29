using System;
using System.Collections.Generic;
using System.Linq;
using Bailiwick.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestPair = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IList<Bailiwick.Models.WordInstance>>;

namespace Bailiwick.Parsers.Test
{
    [TestClass]
    public class ParsingEngineTest
    {
        #region Class Properties
        
        static WordClass Cardinal = WordClasses.Specifics["MC"];
        static WordClass Ordinal = WordClasses.Specifics["MD"];
        static WordInstance Newline = new WordInstance("\n", WordClasses.Specifics["QEN"]);
        static ICorpus corpus;

        #endregion
        #region Unit Test Framework Members
        
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            corpus = new Corpora.CocaCorpus();
        }

        #endregion
        #region Fixture Setup Methods

        static internal ParsingEngine BuildTarget()
        {
            var result = new ParsingEngine(corpus);
            return result;
        }

        static internal List<TestPair> StandardTestSet()
        {
            var pairs = new List<TestPair>();

            // Contraction
            pairs.Add(new TestPair("O'clock", new WordInstance[] { new WordInstance("O'clock") }));
            pairs.Add(new TestPair("L'il", new WordInstance[] { new WordInstance("L'il") }));
            pairs.Add(new TestPair("John's", new WordInstance[] { new WordInstance("John"), new WordInstance("'s") }));
            pairs.Add(new TestPair("Who'd", new WordInstance[] { new WordInstance("Who"), new WordInstance("'d") }));
            pairs.Add(new TestPair("Williams'", new WordInstance[] { new WordInstance("Williams"), new WordInstance("'s", WordClasses.Specifics["GE"]) }));
            pairs.Add(new TestPair("Haven't", new WordInstance[] { new WordInstance("Have"), new WordInstance("n't", WordClasses.Specifics["XX"]) }));
            pairs.Add(new TestPair("We've", new WordInstance[] { new WordInstance("We"), new WordInstance("have") }));
            pairs.Add(new TestPair("Cannot", new WordInstance[] { new WordInstance("Can"), new WordInstance("not", WordClasses.Specifics["XX"]) }));
            pairs.Add(new TestPair("Can't", new WordInstance[] { new WordInstance("Can"), new WordInstance("not", WordClasses.Specifics["XX"]) }));

            // Decimal & Number
            pairs.Add(new TestPair("0", new WordInstance[] { new WordInstance("0", Cardinal) }));
            pairs.Add(new TestPair("19.1", new WordInstance[] { new WordInstance("19.1", Cardinal) }));
            pairs.Add(new TestPair("0.1", new WordInstance[] { new WordInstance("0.1", Cardinal) }));
            pairs.Add(new TestPair("1,000.0001", new WordInstance[] { new WordInstance("1,000.0001", Cardinal) }));
            pairs.Add(new TestPair("-100.15", new WordInstance[] { new WordInstance("-100.15", Cardinal) }));
            pairs.Add(new TestPair("12/5/1920", new WordInstance[] { new WordInstance("12/5/1920", Cardinal) }));
            pairs.Add(new TestPair("1:34", new WordInstance[] { new WordInstance("1:34", Cardinal) }));
            pairs.Add(new TestPair("1:00:00", new WordInstance[] { new WordInstance("1:00:00", Cardinal) }));
            pairs.Add(new TestPair("$100", new WordInstance[] { new WordInstance("$100", Cardinal) }));
            pairs.Add(new TestPair("10%", new WordInstance[] { new WordInstance("10%", Cardinal) }));
            pairs.Add(new TestPair("10.1%", new WordInstance[] { new WordInstance("10.1%", Cardinal) }));
            pairs.Add(new TestPair("2'", new WordInstance[] { new WordInstance("2'", Cardinal) }));
            pairs.Add(new TestPair("2''", new WordInstance[] { new WordInstance("2''", Cardinal) }));
            pairs.Add(new TestPair("2\"", new WordInstance[] { new WordInstance("2\"", Cardinal) }));
            pairs.Add(new TestPair("1/2\"", new WordInstance[] { new WordInstance("1/2\"", Cardinal) }));
            pairs.Add(new TestPair("-2.625'", new WordInstance[] { new WordInstance("-2.625'", Cardinal) }));
            pairs.Add(new TestPair("38°", new WordInstance[] { new WordInstance("38°", Cardinal) }));
            pairs.Add(new TestPair("38.23°", new WordInstance[] { new WordInstance("38.23°", Cardinal) }));

            pairs.Add(new TestPair("1st", new WordInstance[] { new WordInstance("1st", Ordinal) }));
            pairs.Add(new TestPair("2nd", new WordInstance[] { new WordInstance("2nd", Ordinal) }));
            pairs.Add(new TestPair("3rd", new WordInstance[] { new WordInstance("3rd", Ordinal) }));
            pairs.Add(new TestPair("4th", new WordInstance[] { new WordInstance("4th", Ordinal) }));

            // Word
            pairs.Add(new TestPair("test", new WordInstance[] { new WordInstance("test") }));

            return pairs;
        }
        #endregion

        #region Assert Helper Methods

        private WordInstanceComparer Comparer
        {
            get
            {
                return comparer ?? (comparer = new WordInstanceComparer());
            }
        }
        WordInstanceComparer comparer;

        private void VerifyResults(IList<WordInstance> actual, TestPair expected)
        {
            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Value.Count, actual.Count, string.Format("Test failed on '{0}'", expected.Key));
            for (int j = 0; j < actual.Count; j++)
            {
                Assert.IsTrue(Comparer.Equals(expected.Value[j], actual[j]), string.Format("Test failed on '{0}", expected.Key));
            }
        }

        #endregion

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParseEmptyTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParsePeriodTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("end.").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(WordClassType.Punctuation,actual[1].PartOfSpeech.General);
            Assert.AreEqual("QEP", actual[1].PartOfSpeech.Specific);
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParseCRTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("d\rE").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(WordClassType.Punctuation, actual[1].PartOfSpeech.General);
            Assert.AreEqual("QEN", actual[1].PartOfSpeech.Specific);
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParseLFTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("d\nE").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(WordClassType.Punctuation, actual[1].PartOfSpeech.General);
            Assert.AreEqual("QEN", actual[1].PartOfSpeech.Specific);
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParseCRLFTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("d\r\nE").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(WordClassType.Punctuation, actual[1].PartOfSpeech.General);
            Assert.AreEqual("QEN", actual[1].PartOfSpeech.Specific);
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void ParseLFCRTest()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Parse("d\n\rE").ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(WordClassType.Punctuation, actual[1].PartOfSpeech.General);
            Assert.AreEqual("QEN", actual[1].PartOfSpeech.Specific);
        }

        [TestMethod]
        public void FSM_Word_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();

            pairs.Add(new TestPair("", new WordInstance[] { }));
            pairs.Add(new TestPair("The", new WordInstance[] { new WordInstance("The") }));
            pairs.Add(new TestPair("end", new WordInstance[] { new WordInstance("end") }));
            pairs.Add(new TestPair("end.", new WordInstance[] { new WordInstance("end"), new WordInstance(".", WordClasses.Period) }));
            
            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_ApostropheCoding_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();

            pairs.Add(new TestPair("John's", new WordInstance[] { new WordInstance("John"), new WordInstance("'s") }));
            pairs.Add(new TestPair("John\u2019s", new WordInstance[] { new WordInstance("John"), new WordInstance("'s") }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Apostophe_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var singleQuote = new WordInstance("'", WordClasses.SingleQuote);

            // Fake out apostrophes
            pairs.Add(new TestPair("'I", new WordInstance[] { singleQuote, new WordInstance("I") }));
            pairs.Add(new TestPair("O'clock", new WordInstance[] { new WordInstance("O'clock") }));
            pairs.Add(new TestPair("49'ers", new WordInstance[] { new WordInstance("49'ers") }));

            // Compound
            pairs.Add(new TestPair("Bull's-eye", new WordInstance[] { new WordInstance("Bull's-eye") }));

            // Abbreviations
            pairs.Add(new TestPair("'Twas", new WordInstance[] { singleQuote, new WordInstance("Twas") }));
//            pairs.Add(new TestPair("Fo'c's'le", new WordInstance[] { new WordInstance("Fo'c's'le") }));
            pairs.Add(new TestPair("L'il", new WordInstance[] { new WordInstance("L'il") }));

            // Ambiguous middle
            pairs.Add(new TestPair("John's", new WordInstance[] { new WordInstance("John"), new WordInstance("'s") }));
            pairs.Add(new TestPair("Who'd", new WordInstance[] { new WordInstance("Who"), new WordInstance("'d") }));

            // Ambiguous end
            pairs.Add(new TestPair("John'", new WordInstance[] { new WordInstance("John"), singleQuote }));
            pairs.Add(new TestPair("Williams'", new WordInstance[] { new WordInstance("Williams"), new WordInstance("'s", WordClasses.Specifics["GE"]) }));

            // n't
            pairs.Add(new TestPair("Haven't", new WordInstance[] { new WordInstance("Have"), new WordInstance("n't", WordClasses.Specifics["XX"]) }));
            //Don't 
            //Didn't 
            //Couldn't 
            //Wasn't 
            //Wouldn't 
            //Hadn't 
            //Isn't 
            //Doesn't 
            //Haven't 
            //Aren't 
            //Shouldn't 
            //Weren't 
            //Hasn't 
            //Mustn't 

            // Pronoun elision and contraction
            pairs.Add(new TestPair("I'm", new WordInstance[] { new WordInstance("I"), new WordInstance("am") }));
            pairs.Add(new TestPair("I'll", new WordInstance[] { new WordInstance("I"), new WordInstance("will") }));
            pairs.Add(new TestPair("I've", new WordInstance[] { new WordInstance("I"), new WordInstance("have") }));

            pairs.Add(new TestPair("You're", new WordInstance[] { new WordInstance("You"), new WordInstance("are") }));
            pairs.Add(new TestPair("You'll", new WordInstance[] { new WordInstance("You"), new WordInstance("will") }));
            pairs.Add(new TestPair("You've", new WordInstance[] { new WordInstance("You"), new WordInstance("have") }));

            pairs.Add(new TestPair("They'll", new WordInstance[] { new WordInstance("They"), new WordInstance("will") }));
            pairs.Add(new TestPair("They're", new WordInstance[] { new WordInstance("They"), new WordInstance("are") }));

            pairs.Add(new TestPair("We'll", new WordInstance[] { new WordInstance("We"), new WordInstance("will") }));
            pairs.Add(new TestPair("We're", new WordInstance[] { new WordInstance("We"), new WordInstance("are") }));
            pairs.Add(new TestPair("We've", new WordInstance[] { new WordInstance("We"), new WordInstance("have") }));

            // mutated contractions
            pairs.Add(new TestPair("Cannot", new WordInstance[] { new WordInstance("Can"), new WordInstance("not", WordClasses.Specifics["XX"]) }));
            pairs.Add(new TestPair("Can't", new WordInstance[] { new WordInstance("Can"), new WordInstance("not", WordClasses.Specifics["XX"]) }));
            pairs.Add(new TestPair("Ain't", new WordInstance[] { new WordInstance("Be"), new WordInstance("not", WordClasses.Specifics["XX"]) }));
            pairs.Add(new TestPair("Won't", new WordInstance[] { new WordInstance("Will"), new WordInstance("not", WordClasses.Specifics["XX"]) }));
            // Gonna
            // Gotta
            // Wanna

            // Measurement
            // 12''
            // 45'

            // Dropped G
            pairs.Add(new TestPair("bein'", new WordInstance[] { new WordInstance("being") }));
            pairs.Add(new TestPair("considerin'", new WordInstance[] { new WordInstance("considering") }));
            pairs.Add(new TestPair("durin'", new WordInstance[] { new WordInstance("during") }));
            pairs.Add(new TestPair("nothin'", new WordInstance[] { new WordInstance("nothing") }));
            pairs.Add(new TestPair("somethin'", new WordInstance[] { new WordInstance("something") }));
            pairs.Add(new TestPair("comin'", new WordInstance[] { new WordInstance("coming") }));
            pairs.Add(new TestPair("countin'", new WordInstance[] { new WordInstance("counting") }));
            pairs.Add(new TestPair("dryin'", new WordInstance[] { new WordInstance("drying") }));
            pairs.Add(new TestPair("givin'", new WordInstance[] { new WordInstance("giving") }));
            pairs.Add(new TestPair("goin'", new WordInstance[] { new WordInstance("going") }));
            pairs.Add(new TestPair("GOIN'", new WordInstance[] { new WordInstance("GOING") }));
            pairs.Add(new TestPair("Goin'", new WordInstance[] { new WordInstance("Going") }));
            pairs.Add(new TestPair("herdin'", new WordInstance[] { new WordInstance("herding") }));
            pairs.Add(new TestPair("prayin'", new WordInstance[] { new WordInstance("praying") }));
            pairs.Add(new TestPair("runnin'", new WordInstance[] { new WordInstance("running") }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Number_CardinalsTests()
        {
            // Setup
            var target = BuildTarget();
            var numbers = new List<string>();

            // Basic
            numbers.Add("0");
            numbers.Add("1");
            numbers.Add("19");
            numbers.Add("100");

            // Signed
            numbers.Add("-1");
            numbers.Add("+1");

            // Decimal/Formats
            numbers.Add("19.1");
            numbers.Add("0.1");
            numbers.Add("1,000");
            numbers.Add("1,000.0001");
            numbers.Add("1,000,000");
            numbers.Add("-100.15");

            // Dates
            numbers.Add("12/5/1920");

            // Time
            numbers.Add("1:34");
            numbers.Add("10:23");
            numbers.Add("1:00:00");
            numbers.Add("-1:43");

            // Currency
            numbers.Add("$100");
            numbers.Add("-$100");
            numbers.Add("2¢");
            numbers.Add("£100.0");
            numbers.Add("100£");
            numbers.Add("100.00£");
            numbers.Add("99¤");
            numbers.Add("10,000¥");
            numbers.Add("88,00€");

            // Percent
            numbers.Add("10%");
            numbers.Add("10.1%");

            // Measurement
            numbers.Add("2'");
            numbers.Add("2''");
            numbers.Add("2\"");
            numbers.Add("1/2\"");
            numbers.Add("-2.625'");
            numbers.Add("38°");
            numbers.Add("38.23°");

            var pairs = new List<TestPair>();
            foreach(var c in numbers)
                pairs.Add(new TestPair(c, new WordInstance[] { new WordInstance(c, WordClasses.Specifics["MC"]) }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Number_OrdinalsTests()
        {
            // Setup
            var target = BuildTarget();
            var numbers = new List<string>();

            // Ordinals - MD
            numbers.Add("1st");
            numbers.Add("2nd");
            numbers.Add("3rd");
            numbers.Add("4th");
            numbers.Add("11th");
            numbers.Add("12th");
            numbers.Add("21st");
            numbers.Add("1,000,000th");

            var pairs = new List<TestPair>();
            foreach (var o in numbers)
                pairs.Add(new TestPair(o, new WordInstance[] { new WordInstance(o, WordClasses.Specifics["MD"]) }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Number_BadTests()
        {
            // Setup
            var target = BuildTarget();
            var bad = new List<string>();

            // TODO - Handle correctly
            // bad.Add("192.168.1.1");

            // Bad numbers
            bad.Add("14th4");
            bad.Add("2.99mm");
            bad.Add("9mm");
            bad.Add("1960's");

            var pairs = new List<TestPair>();
            foreach (var b in bad)
                pairs.Add(new TestPair(b, new WordInstance[] { new WordInstance(b, WordClasses.Unclassified) }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void FSM_Number_YearsTest()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var eos = new WordInstance(".", WordClasses.Period);
            var nnt1 = WordClasses.Specifics["NNT1"];
            var nnt2 = WordClasses.Specifics["NNT2"];

            // In range
            pairs.Add(new TestPair("2010", new WordInstance[] { new WordInstance("2010", nnt1) }));
            pairs.Add(new TestPair("1912", new WordInstance[] { new WordInstance("1912", nnt1) }));
            pairs.Add(new TestPair("1776", new WordInstance[] { new WordInstance("1776", nnt1) }));
            pairs.Add(new TestPair("1000", new WordInstance[] { new WordInstance("1000", nnt1) }));
            pairs.Add(new TestPair("1990s", new WordInstance[] { new WordInstance("1990s", nnt2) }));

            // Out of range
            pairs.Add(new TestPair("3000", new WordInstance[] { new WordInstance("3000", Cardinal) }));
            pairs.Add(new TestPair("999", new WordInstance[] { new WordInstance("999", Cardinal) }));

            // Not right
            pairs.Add(new TestPair("2,010", new WordInstance[] { new WordInstance("2,010", Cardinal) }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void FSM_Number_TrailingPunctuationTest()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var eos = new WordInstance(".", WordClasses.Period);
            var nnt1 = WordClasses.Specifics["NNT1"];
            var nnt2 = WordClasses.Specifics["NNT2"];

            pairs.Add(new TestPair("2010.", new WordInstance[] { new WordInstance("2010", nnt1), eos }));
            pairs.Add(new TestPair("2,010.", new WordInstance[] { new WordInstance("2,010", Cardinal), eos }));
            pairs.Add(new TestPair("0.5.", new WordInstance[] { new WordInstance("0.5", Cardinal), eos }));
            pairs.Add(new TestPair("0.5%.", new WordInstance[] { new WordInstance("0.5%", Cardinal), eos }));
            pairs.Add(new TestPair("1950s.", new WordInstance[] { new WordInstance("1950s", nnt2), eos }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Comma_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();
            var comma = new WordInstance(",", WordClasses.Comma);
            
            // Execute
            WordInstance[] actual = null;
            List<WordInstance> list;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                // Before
                list = new List<WordInstance>();
                list.Add(comma);
                list.AddRange(pairs[i].Value);
                testPair = new TestPair("," + pairs[i].Key, list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);

                // After
                list = pairs[i].Value.ToList();
                list.Add(comma);
                testPair = new TestPair(pairs[i].Key + ",", list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);

                // In-between
                list = new List<WordInstance>();
                list.Add(Newline);
                list.Add(comma);
                list.AddRange(pairs[i].Value);
                testPair = new TestPair("\n," + pairs[i].Key, list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);
            }
        }

        [TestMethod]
        public void FSM_DoubleQuote_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();
            
            var quotes = new string[] 
            { 
                "\"", "\u201c", "\u201d", "\u201f"
            };

            // Execute
            WordInstance[] actual = null;
            WordInstance expected;
            List<WordInstance> list;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i].Value[0].PartOfSpeech.Specific.StartsWith("M"))
                    continue;

                for (int j = 0; j < quotes.Length; j++)
                {
                    expected = new WordInstance(quotes[j], WordClasses.DoubleQuote);

                    // Before
                    list = new List<WordInstance>();
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair(quotes[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // After
                    list = pairs[i].Value.ToList();
                    list.Add(expected);
                    testPair = new TestPair(pairs[i].Key + quotes[j], list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // In-between
                    list = new List<WordInstance>();
                    list.Add(Newline);
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair("\n" + quotes[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);
                }
            }
        }

        [TestMethod]
        public void FSM_SingleQuote_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();

            var quotes = new string[] 
            { 
                "\'", "\u2018", "\u2019", "\u201b"
            };

            // Execute
            WordInstance[] actual = null;
            WordInstance expected;
            List<WordInstance> list;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i].Value[0].PartOfSpeech.Specific.StartsWith("M"))
                    continue;

                for (int j = 0; j < quotes.Length; j++)
                {
                    expected = new WordInstance(quotes[j], WordClasses.SingleQuote);

                    // Before
                    list = new List<WordInstance>();
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair(quotes[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // After
                    list = pairs[i].Value.ToList();
                    list.Add(expected);
                    testPair = new TestPair(pairs[i].Key + quotes[j], list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // In-between
                    list = new List<WordInstance>();
                    list.Add(Newline);
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair("\n" + quotes[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);
                }
            }
        }

        [TestMethod]
        public void FSM_Semicolon_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();
            var semicolon = new WordInstance(";", WordClasses.Semicolon);

            // Execute
            WordInstance[] actual = null;
            List<WordInstance> list;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                // Before
                list = new List<WordInstance>();
                list.Add(semicolon);
                list.AddRange(pairs[i].Value);
                testPair = new TestPair(";" + pairs[i].Key, list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);

                // After
                list = pairs[i].Value.ToList();
                list.Add(semicolon);
                testPair = new TestPair(pairs[i].Key + ";", list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);

                // In-between
                list = new List<WordInstance>();
                list.Add(Newline);
                list.Add(semicolon);
                list.AddRange(pairs[i].Value);
                testPair = new TestPair("\n;" + pairs[i].Key, list);
                actual = target.Parse(testPair.Key).ToArray();
                VerifyResults(actual, testPair);
            }
        }

        [TestMethod]
        public void FSM_Hyphenation_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var hyphen = new WordInstance("-", WordClasses.Hyphen);
            var range = WordClasses.Specifics["MCMC"];

            // Compound word
            pairs.Add(new TestPair("fast-break", new WordInstance[] { new WordInstance("fast-break") }));
            pairs.Add(new TestPair("anti-aircraft", new WordInstance[] { new WordInstance("anti-aircraft") }));
            pairs.Add(new TestPair("fishing-tackle", new WordInstance[] { new WordInstance("fishing-tackle") }));

            // Compound with trickiness
            pairs.Add(new TestPair("small-scale.", new WordInstance[] { new WordInstance("small-scale"), new WordInstance(".", WordClasses.Period) }));
            pairs.Add(new TestPair("self-reliance,", new WordInstance[] { new WordInstance("self-reliance"), new WordInstance(",", WordClasses.Comma) }));

            // Punctuation
            pairs.Add(new TestPair("made--over", new WordInstance[] { new WordInstance("made"), hyphen, new WordInstance("over") }));
            pairs.Add(new TestPair("mild-mannered--health", new WordInstance[] { new WordInstance("mild-mannered"), hyphen, new WordInstance("health") }));
            pairs.Add(new TestPair("1--pounded", new WordInstance[] { new WordInstance("1", WordClasses.Number), hyphen, new WordInstance("pounded") }));

            // Range
            pairs.Add(new TestPair("55-56", new WordInstance[] { new WordInstance("55-56", range) }));
            pairs.Add(new TestPair("1955-6", new WordInstance[] { new WordInstance("1955-6", range) }));
            pairs.Add(new TestPair("1955-56", new WordInstance[] { new WordInstance("1955-56", range) }));
            pairs.Add(new TestPair("1955-1956", new WordInstance[] { new WordInstance("1955-1956", range) }));
            // '55-'56 

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            }
        }

        [TestMethod]
        public void FSM_Hyphen_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();

            var hyphens = new string[] 
            { 
                "-", "\u2012", "\u2013", "\u2014", "\u2015", "--" 
            };

            // Execute
            WordInstance[] actual = null;
            WordInstance expected;
            List<WordInstance> list;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i].Value[0].PartOfSpeech.Specific.StartsWith("M"))
                    continue;

                for (int j = 0; j < hyphens.Length; j++)
                {
                    expected = new WordInstance("-", WordClasses.Hyphen);

                    // Before
                    list = new List<WordInstance>();
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair(hyphens[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // After
                    list = pairs[i].Value.ToList();
                    list.Add(expected);
                    testPair = new TestPair(pairs[i].Key + hyphens[j], list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // In-between
                    list = new List<WordInstance>();
                    list.Add(Newline);
                    list.Add(expected);
                    list.AddRange(pairs[i].Value);
                    testPair = new TestPair("\n" + hyphens[j] + pairs[i].Key, list);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);
                }
            }
        }

        [TestMethod]
        public void FSM_Markers_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = StandardTestSet();

            // "Junk"
            var markers = new char[] 
            { 
                '[', ']', '(', ')', '{', '}', '⟨', '⟩', ':', '~', '@', '#', '^', '*', '=', '|'
                ,'\u2026' //ellipsis
                ,'/', '\\'
                , '\u2022', '\u2023', '\u25e6', '\u2043', '\u2219' // bullets
                , '\u2033' //doule-prime or ditto
            };

            // Execute
            WordInstance[] actual;
            TestPair testPair;

            for (int i = 0; i < pairs.Count; i++)
            {
                for (int j = 0; j < markers.Length; j++)
                {
                    if (markers[j] == '\u2033' && pairs[i].Value[0].PartOfSpeech.Specific.StartsWith("M"))
                        continue;

                    // Before
                    testPair = new TestPair(markers[j] + pairs[i].Key, pairs[i].Value);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // After
                    testPair = new TestPair(pairs[i].Key + markers[j], pairs[i].Value);
                    actual = target.Parse(testPair.Key).ToArray();
                    VerifyResults(actual, testPair);

                    // In-between
                    //pairs.Add(new TestPair("\nbut", new WordInstance[] { newline, new WordInstance("but") }));
                }
            }
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void FSM_Period_Tests()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var period = new WordInstance(".", WordClasses.Period);

            // Words
            pairs.Add(new TestPair("it.", new WordInstance[] { new WordInstance("it"), period }));
            pairs.Add(new TestPair("place.", new WordInstance[] { new WordInstance("place"), period }));
            pairs.Add(new TestPair("IT.", new WordInstance[] { new WordInstance("IT"), period }));
            
            // Titles
            pairs.Add(new TestPair("Mr.", new WordInstance[] { new WordInstance("Mr.", "mister") }));
            pairs.Add(new TestPair("Mrs.", new WordInstance[] { new WordInstance("Mrs.", "madam") }));
            pairs.Add(new TestPair("Dr.", new WordInstance[] { new WordInstance("Dr.", "doctor") }));

            // Capitalized Nouns that happen to end a sentence
            pairs.Add(new TestPair("America.", new WordInstance[] { new WordInstance("America"), period }));

            // Abbreviations
            pairs.Add(new TestPair("Prof.", new WordInstance[] { new WordInstance("Prof.", "professor") }));
            pairs.Add(new TestPair("Gen.", new WordInstance[] { new WordInstance("Gen.", "general") }));
            pairs.Add(new TestPair("Rep.", new WordInstance[] { new WordInstance("Rep.", "representative") }));
            pairs.Add(new TestPair("Sen.", new WordInstance[] { new WordInstance("Sen.", "senator") }));
            pairs.Add(new TestPair("St.", new WordInstance[] { new WordInstance("St.", "st.") }));
            pairs.Add(new TestPair("Sr.", new WordInstance[] { new WordInstance("Sr.", "senior") }));
            pairs.Add(new TestPair("Jr.", new WordInstance[] { new WordInstance("Jr.", "junior") }));

            // Common Latin
            pairs.Add(new TestPair("etc.", new WordInstance[] { new WordInstance("etc.", "etcetera") }));
            pairs.Add(new TestPair("al.", new WordInstance[] { new WordInstance("al.", "al.") }));
            pairs.Add(new TestPair("vs.", new WordInstance[] { new WordInstance("vs.", "versus") }));

            // Initialisms / Acronyms
            pairs.Add(new TestPair("I.T.", new WordInstance[] { new WordInstance("I.T.") }));
            pairs.Add(new TestPair("A.M.", new WordInstance[] { new WordInstance("A.M.") }));
            pairs.Add(new TestPair("U.S.S.R.", new WordInstance[] { new WordInstance("U.S.S.R.") }));

            // List headers
            pairs.Add(new TestPair("A.", new WordInstance[] { new WordInstance("A"), period }));
            pairs.Add(new TestPair("B.", new WordInstance[] { new WordInstance("B"), period }));
            pairs.Add(new TestPair("a.", new WordInstance[] { new WordInstance("a"), period }));
            pairs.Add(new TestPair("b.", new WordInstance[] { new WordInstance("b"), period }));
            pairs.Add(new TestPair("I.", new WordInstance[] { new WordInstance("I"), period }));
            pairs.Add(new TestPair("II.", new WordInstance[] { new WordInstance("II"), period }));
            pairs.Add(new TestPair("i.", new WordInstance[] { new WordInstance("i"), period }));
            pairs.Add(new TestPair("ii.", new WordInstance[] { new WordInstance("ii"), period }));
            pairs.Add(new TestPair("iv.", new WordInstance[] { new WordInstance("iv"), period }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            };
        }

        [TestMethod]
        [Owner("jefar@us.ibm.com")]
        public void FSM_RPS_Defect_Test()
        {
            // Setup
            var target = BuildTarget();
            var pairs = new List<TestPair>();
            var period = new WordInstance(".", WordClasses.Period);
            var cardinal = WordClasses.Specifics["MC"];
            var newline = new WordInstance("\n", WordClasses.Specifics["QEN"]);

            // Words
            pairs.Add(new TestPair("for.\r\n3.", new WordInstance[] { new WordInstance("for"), period, newline, new WordInstance("3", cardinal), new WordInstance(".", period) }));
            pairs.Add(new TestPair("Activity.", new WordInstance[] { new WordInstance("Activity"), period }));
            pairs.Add(new TestPair("Docket.", new WordInstance[] { new WordInstance("Docket"), period }));

            // Execute
            for (int i = 0; i < pairs.Count; i++)
            {
                var actual = target.Parse(pairs[i].Key).ToArray();

                // Assert
                VerifyResults(actual, pairs[i]);
            };
        }
		  
    }
}
