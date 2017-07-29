using System.Linq;

namespace Bailiwick.Models
{
    public class PhrasalVerb
    {
        public PhrasalVerb(string lemma, params string[] particles)
        {
            Lemma = lemma;
            Particles = particles.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        public string Lemma { get; set; }

        public string[] Particles { get; private set; }	
    }
}
