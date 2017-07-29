using System;
using System.Collections.Generic;
using Bailiwick.Models;

namespace Bailiwick.UI
{
    public interface IKwicStrategy
    {
        /// <summary>
        /// The human-readable name of the strategy
        /// </summary>
        string Title {get;}

        /// <summary>
        /// Extracts the appropriate list of key words from a distribution
        /// </summary>
        /// <param name="distribution">The list of words encountered</param>
        /// <returns>An enumeration of key words supported by this strategy</returns>
        IEnumerable<string> KeyWords(GlossDistribution distribution);

        /// <summary>
        /// Creates a function from the given key word that can be used by the LINQ routines to filter words
        /// </summary>
        /// <param name="keyWord">The selected keyword</param>
        /// <returns>A function suitable for use in a LINQ Where clause</returns>
        Func<WordInstance, bool> BuildFilter(string keyWord);
    }
}
