using System;
using Bailiwick.Models;

namespace Bailiwick.Thoughts.States
{
    internal class EOS : IState
    {
        public bool Ready
        {
            get { return true; }
        }

        public void Enter(IContext c, WordInstance wi)
        {
        }

        public void OnWord(IContext c, WordInstance wi)
        {
            c.Next.Enqueue(wi);
        }

        public override int GetHashCode()
        {
            return 3;
        }

    }
}
