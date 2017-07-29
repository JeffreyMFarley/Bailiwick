using Bailiwick.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Bailiwick.DAL
{
    public class PhrasalVerbFormatter : IFormatter
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

        #region PhrasalVerbFormatter members

        public IEnumerable<PhrasalVerb> Deserialize(Stream stream)
        {
            char[] seps = new char[] { '\t' };
            string[] tokens;
            PhrasalVerb phrasal;

            using (var streamIn = new StreamReader(stream))
            {
                // Skip header
                streamIn.ReadLine();

                while (!streamIn.EndOfStream)
                {
                    tokens = streamIn.ReadLine().Split(seps, StringSplitOptions.None);
                    Debug.Assert(tokens.Length == 4);

                    phrasal = new PhrasalVerb(tokens[0], tokens[1], tokens[2], tokens[3]);
                    yield return phrasal;
                }

            }
        }

        public void Serialize(Stream stream, IEnumerable<PhrasalVerb> verbs)
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
            this.Serialize(stream, (IEnumerable<PhrasalVerb>)graph);
        }

        #endregion
    }
}
