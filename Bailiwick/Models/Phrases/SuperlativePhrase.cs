using System.Collections.Generic;
using System.Linq;

namespace Bailiwick.Models.Phrases
{
    public class SuperlativePhrase : NounPhrase
    {
        public override WordInstance Head
        {
            get
            {
                return Adjuncts.Last(x => x.GeneralWordClass == WordClassType.Adjective);
            }
        }
    }
}
