using System;
using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders
{
    internal class TracingContext : IContext
    {
        public TracingContext(IContext realImplementation)
        {
            Real = realImplementation;
        }

        IContext Real { get; set; }

        internal void Dump(string method)
        {
            var s = string.Join("\t", Real.Previous.GetType().Name,
                                      Real.State.GetType().Name,
                                      Real.Current,
                                      Real.Accepted.Count,
                                      Real.Buffer.Count,
                                      method);

            Console.WriteLine(s);
        }

        #region IContext Members

        public IState State
        {
            get { return Real.State; }
        }

        public IState Previous
        {
            get { return Real.Previous; }
        }

        public void ResolvePsuedoState(IState state, bool includeBuffer)
        {
            Dump(string.Format("ResolvePsuedoState({0}, {1})", state.GetType().Name, includeBuffer));
            Real.ResolvePsuedoState(state, includeBuffer);
        }

        public List<WordInstance> Accepted
        {
            get { return Real.Accepted; }
        }

        public List<WordInstance> Buffer
        {
            get { return Real.Buffer; }
        }

        public WordInstance Current
        {
            get { return Real.Current; }
        }

        public void AcceptCurrent()
        {
            Dump("AcceptCurrent");
            Real.AcceptCurrent();
        }

        public void BufferCurrent()
        {
            Dump("BufferCurrent");
            Real.BufferCurrent();
        }

        public void RejectCurrent()
        {
            Dump("RejectCurrent");
            Real.RejectCurrent();
        }

        public void PushAccepted()
        {
            Dump("PushAccepted");
            Real.PushAccepted();
        }

        public void RejectAccepted()
        {
            Dump("RejectAccepted");
            Real.RejectAccepted();
        }

        public void AcceptBuffer()
        {
            Dump("AcceptBuffer");
            Real.AcceptBuffer();
        }

        public IConvert TryConvert
        {
            get 
            {
                Dump("TryConvert");
                return Real.TryConvert; 
            }
        }

        public IConvert ForceConvert
        {
            get 
            {
                Dump("ForceConvert");
                return Real.ForceConvert; 
            }
        }

        #endregion
    }
}
