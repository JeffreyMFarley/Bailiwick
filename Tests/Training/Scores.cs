using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Bailiwick.Models;
using Esoteric.Collections;

namespace Bailiwick.Tests.Training
{
    internal class Scores : Dictionary<string, float>
    {
        #region Upbuilding

        internal static Scores Load(string pathName)
        {
            if( !File.Exists(pathName) )
                return null;

            char[] seps = new char[] { '\t' };
            string line;
            string[] tokens;

            var instance = new Scores();
            using (var inFile = new FileStream(pathName, FileMode.Open))
            using (var streamIn = new StreamReader(inFile))
            {
                // Skip header
                streamIn.ReadLine();

                while (!streamIn.EndOfStream)
                {
                    line = streamIn.ReadLine();
                    if (!string.IsNullOrEmpty(line)) 
                    {
                        tokens = line.Split(seps, StringSplitOptions.None);
                        instance[tokens[0]] = Convert.ToSingle(tokens[1]);
                    }
                }
            }

            return instance;
        }

        #endregion
        #region Scoring Methods

        internal static string Grade(float score)
        {
            if (score < 0) return "?";
            else if (score > 99.9) return "A";
            else if (score > 89.9) return "B";
            else if (score > 79.9) return "C";
            else if (score > 69.9) return "D";
            else return "F";
        }

        internal bool AllPassed()
        {
            return Values.All(x => x >= 80.0);
        }

        internal static float Round(float x)
        {
            return Convert.ToSingle(Math.Round((decimal)x, 2));
        }

        #endregion
        #region Output

        internal void DumpGrades(bool listKeys)
        {
            Console.WriteLine("\n------- Grades ----------");
            var gen = from kvp in this
                      group kvp by Grade(kvp.Value) into g
                      select g;

            foreach (var g0 in gen.OrderBy(x => x.Key))
            {
                var count = g0.Count();
                Console.Write(g0.Key); Console.Write('\t');
                Console.Write(count); Console.Write('\t');
                Console.Write((count * 100) / this.Count); Console.Write("%\t");
                if (listKeys)
                    Console.WriteLine(string.Join(", ", g0.Take(20).Select(x => x.Key)));
                else
                    Console.WriteLine();
            }
        }

        internal void Save(string pathName)
        {
            using (var outFile = new FileStream(pathName, FileMode.Create))
            using (var writer = new StreamWriter(outFile))
            {
                writer.WriteLine("Key\tValue");
                foreach (var k in Keys.OrderBy(x => x))
                {
                    writer.WriteLine(string.Format("{0}\t{1}", k, Round(this[k])));
                }
            }
        }

        #endregion
        #region Comparison

        internal void Compare(Scores that)
        {
            if( that == null )
                return;

            var better = new StringDistribution();
            var worse = new StringDistribution();
            int missing=0;
            string thatGrade, thisGrade;
            float score;

            foreach (var k in Keys.OrderBy(x => x))
            {
                if (!that.TryGetValue(k, out score))
                    missing += 1;

                else
                {
                    thisGrade = Grade(Round(this[k]));
                    thatGrade = Grade(Round(score));
                    var compare = string.Compare(thisGrade, thatGrade);
                    if( compare < 0 )
                        better.Increment(thisGrade);
                    else if( compare > 0 )
                        worse.Increment(thisGrade);
                }
            }

            string[] cols = {"?", "A", "B", "C", "D", "F"};

            Console.WriteLine("\n---- Baseline Comparison -----\n");
            Console.WriteLine("Grade\tBetter\tWorse");
            foreach (var col in cols)
            {
                var b = (better.ContainsKey(col)) ? string.Format("{0}", better[col]) : " ";
                var w = (worse.ContainsKey(col)) ? string.Format("{0}", worse[col]) : " ";
                
                Console.WriteLine(string.Format("{0}\t{1}\t{2}", col, b, w));
            }
            Console.WriteLine(string.Format("Total\t{0}\t{1}", better.Sum, worse.Sum));
            Console.WriteLine(string.Format("\nMissing\t{0}", missing));
            Console.WriteLine(string.Format("Total\t{0}", Count));
        }

        internal IEnumerable<Tuple<string, string>> PerformedWorse(Scores that)
        {
            if (that == null)
                yield break;

            string thatGrade, thisGrade;
            float score;

            foreach (var k in Keys.OrderBy(x => x))
            {
                if (that.TryGetValue(k, out score))
                {
                    thisGrade = Grade(Round(this[k]));
                    thatGrade = Grade(Round(score));
                    var compare = string.Compare(thisGrade, thatGrade);
                    if (compare > 0)
                        yield return new Tuple<string, string>(k, thisGrade);
                }
            }
        }

        #endregion
    }
}
