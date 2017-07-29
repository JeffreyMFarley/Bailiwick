using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bailiwick.Models.Tests
{
    [TestClass]
    public class WordCaseStatusTest
    {
        #region Class Properties
        #endregion

        #region Unit Test Framework Members
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            // Create the Test Doubles
        }
        #endregion

        #region Fixture Setup Methods
        static internal WordCaseStatus BuildTarget()
        {
            var result = new WordCaseStatus();

            return result;
        }
        #endregion

        [TestMethod]
        public void CheckTest_Empty()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            target.Check("");

            // Assert
            Assert.IsNull(target.UpperCase);
            Assert.IsNull(target.LowerCase);
            Assert.IsNull(target.TitleCase);
        }

        [TestMethod]
        public void CheckTest_Number()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            target.Check("999");

            // Assert
            Assert.IsNull(target.UpperCase);
            Assert.IsNull(target.LowerCase);
            Assert.IsNull(target.TitleCase);
        }

        [TestMethod]
        public void CheckTest_Uppercase()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            target.Check("ALL CAPS");

            // Assert
            Assert.IsTrue(target.UpperCase ?? false);
            Assert.IsFalse(target.LowerCase ?? true);
            Assert.IsFalse(target.TitleCase ?? true);
        }

        [TestMethod]
        public void CheckTest_Lowercase()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            target.Check("all caps");

            // Assert
            Assert.IsFalse(target.UpperCase ?? true);
            Assert.IsTrue(target.LowerCase ?? false);
            Assert.IsFalse(target.TitleCase ?? true);
        }

        [TestMethod]
        public void CheckTest_Titlecase()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            target.Check(" Title");

            // Assert
            Assert.IsFalse(target.UpperCase ?? true);
            Assert.IsFalse(target.LowerCase ?? true);
            Assert.IsTrue(target.TitleCase ?? false);
        }

        [TestMethod]
        public void ApplyTest_Empty()
        {
            // Setup
            var target = BuildTarget();

            // Execute
            var actual = target.Apply("");

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void ApplyTest_Number()
        {
            // Setup
            var target = BuildTarget();
            target.UpperCase = true;
            var value = "999";

            // Execute
            var actual = target.Apply(value);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(value, actual);
        }

        [TestMethod]
        public void ApplyTest_Uppercase()
        {
            // Setup
            var target = BuildTarget();
            target.UpperCase = true;
            var value = " ThIs";

            // Execute
            var actual = target.Apply(value);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(" THIS", actual);
        }

        [TestMethod]
        public void ApplyTest_Lowercase()
        {
            // Setup
            var target = BuildTarget();
            target.LowerCase = true;
            var value = " ThIs";

            // Execute
            var actual = target.Apply(value);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(" this", actual);
        }

        [TestMethod]
        public void ApplyTest_Titlecase()
        {
            // Setup
            var target = BuildTarget();
            target.TitleCase = true;
            var value = "this";

            // Execute
            var actual = target.Apply(value);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual("This", actual);
        }
    }
}
