using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Bailiwick.Models;

namespace Bailiwick.Tests.Training
{
    [CollectionDataContract]
    public class Document : Dictionary<string, TaggedWord[]> { }

    [DataContract]
    public class TaggedWord
    {
        public TaggedWord(string word, string tag)
        {
            Tag = tag;
            Word = word;
        }

        public TaggedWord(string word, object[] tags)
        {
            Word = word;
            TagSet = tags.Cast<string>().ToArray();
        }

        [DataMember(Name = "t", IsRequired = false, EmitDefaultValue = false)]
        public string Tag
        {
            get;
            private set;
        }

        [DataMember(Name = "w", IsRequired=false,EmitDefaultValue=false)]
        public string Word
        {
            get;
            private set;
        }

        [DataMember(Name = "ts", IsRequired = false, EmitDefaultValue = false)]
        public string[] TagSet
        {
            get;
            private set;
        }

        public IEnumerable<WordClass> Tags()
        {
            if( string.IsNullOrEmpty(Tag) )
                foreach(var t in TagSet)
                    yield return WordClasses.Specifics[t];
            else
                yield return WordClasses.Specifics[Tag];
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Word, string.Join(",", Tags().Select(x => x.Specific)));
        }
    }
}
