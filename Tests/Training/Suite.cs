using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Bailiwick.Tests.Training
{
    public class Suite
    {
        private Dictionary<string, object[]> LoadEmbeddedResource()
        {
            var type = GetType();
            var assm = type.Assembly;
            var name = type.Namespace + ".BrownCorpusClaws.json";
            var loader = new JavaScriptSerializer();

            using (var stream = assm.GetManifestResourceStream(name))
            using (var reader = new System.IO.StreamReader(stream))
            {
                var s = reader.ReadToEnd();
                loader.MaxJsonLength = s.Length + 10;
                return loader.Deserialize<Dictionary<string, object[]>>(s);
            }
        }

        private Document Convert(Dictionary<string, object[]> src)
        {
            var d = new Document();
            foreach (var k in src.Keys.OrderBy(x => x))
            {
                var twl = new List<TaggedWord>();
                foreach (Dictionary<string, object> input in src[k])
                {
                    TaggedWord tw;
                    if (input.ContainsKey("w"))
                        tw = new TaggedWord(input["w"] as string, input["t"] as string);
                    else
                        tw = new TaggedWord(input["mw"] as string, input["ts"] as object[]);
                    twl.Add(tw);
                }
                d[k] = twl.ToArray();
            }

            return d;
        }

        public Document Sentences
        {
            get
            {
                if (_document == null) 
                {
                    _document = Convert(LoadEmbeddedResource());
                }

                return _document;
            }
        }
        Document _document;
    }
}
