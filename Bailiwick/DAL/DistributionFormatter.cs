using Bailiwick.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bailiwick.DAL
{
    public class DistributionFormatter : IFormatter
    {
        #region IFormatter Properties
        /// <summary>
        /// Used to implment version control.  Converts a request for one type to another
        /// </summary>
        public SerializationBinder Binder
        {
            get;
            set;
        }

        /// <summary>
        /// Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.
        /// </summary>
        public StreamingContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Selects which ISerializationSurrogate is used to serialize/deserailize a type
        /// </summary>
        public ISurrogateSelector SurrogateSelector
        {
            get;
            set;
        }
        #endregion

        #region DistributionFormatter members

        public IEnumerable<Frequency> Deserialize(Stream stream)
        {
            char[] seps = new char[] {'\t'};
            string[] tokens;
            long count;
            WordClass wordClass;

            using (var freqIn = new StreamReader(stream))
            {
                // Skip header
                freqIn.ReadLine();

                while (!freqIn.EndOfStream) 
                {
                    tokens = freqIn.ReadLine().Split(seps, StringSplitOptions.None);
                    Debug.Assert(tokens.Length == 4);

                    wordClass = WordClasses.Specifics[tokens[2].ToUpper()];
                    count = Int64.Parse(tokens[3]);

                    yield return new Frequency(tokens[0], wordClass, tokens[1], count);
                }

            }
        }

        public void Serialize(Stream stream, IEnumerable<Frequency> frequency)
        {
            using (var freqOut = new StreamWriter(stream, Encoding.BigEndianUnicode))
            {
                freqOut.WriteLine("Word\tLemma\tPOS\tCount");
                foreach (var f in frequency)
                {
                    freqOut.WriteLine("{0}\t{1}\t{2}\t{3}", f.Normalized, f.Lemma, f.PartOfSpeech.Specific, f.Count);
                }
            }
        }

        #endregion

        #region IFormatter Methods

        object IFormatter.Deserialize(Stream stream)
        {
            return this.Deserialize(stream).ToArray();
        }

        void IFormatter.Serialize(Stream stream, object graph)
        {
            this.Serialize(stream, (IEnumerable<Frequency>)graph);
        }

        #endregion
    }
}
