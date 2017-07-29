using Bailiwick.Models;
using System.Linq;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class PsuedoConjunction : PsuedoBase
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Conjunction; }
        }

        public override void OnEntry(IContext c)
        {
            c.BufferCurrent();
        }

        public override IState OnPunctuation(IContext c)
        {
            if (c.Current.IsEndOfSentence())
            {
                if (c.Buffer.All(x => c.TryConvert.ToAgent(x)))
                { 
                    c.ResolvePsuedoState(c.Previous, true);
                    return BaseState.Initial.OnPunctuation(c);
                }
            }
            
            return base.OnPunctuation(c);
        }
    }
}
