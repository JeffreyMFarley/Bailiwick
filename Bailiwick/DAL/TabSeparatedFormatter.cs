using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Bailiwick.DAL
{
    public class TabSeparatedFormatter : IFormatter
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

        #region TabSeparatedFormatter members

        public IEnumerable<string[]> Deserialize(Stream stream)
        {
            char[] seps = new char[] { '\t' };
            string line;
            string[] tokens;

            using (var streamIn = new StreamReader(stream))
            {
                // Skip header
                streamIn.ReadLine();

                while (!streamIn.EndOfStream)
                {
                    line = streamIn.ReadLine();
                    if (!string.IsNullOrEmpty(line)) 
                    {
                        tokens = line.Split(seps, StringSplitOptions.None);
                        yield return tokens;
                    }
                }

            }
        }

        public void Serialize(Stream stream, IEnumerable<string[]> tokens)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFormatter Methods

        object IFormatter.Deserialize(Stream stream)
        {
            return this.Deserialize(stream).ToArray();
        }

        void IFormatter.Serialize(Stream stream, object graph)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
