using System;
using System.Collections.Generic;
using Bailiwick.Models;
using Bailiwick.Morphology.States;

namespace Bailiwick.Morphology
{
    internal class StateFactory
    {
        #region Static Methods
        private static readonly Lazy<StateFactory> _factory = new Lazy<StateFactory>(Build, true);

        private static StateFactory Build()
        {
            return new StateFactory();
        }

        internal static IState GetState(IGloss g)
        {
            var factory = _factory.Value;

            IState result;
            if( factory.SpecificWords.TryGetValue(g.Lemma, out result) )
                return result;

            if (factory.SpecificTags.TryGetValue(g.PartOfSpeech.Specific, out result))
                return result;

            if (factory.WordClassStates.TryGetValue(g.PartOfSpeech.General, out result))
                return result;

            return factory.NullState;
        } 
        #endregion

        #region Constructor
        internal StateFactory()
        {
            var pps = new PronounPersonalSubjective();

            SpecificTags["DDQ"] = new Determiner_Wh();
            SpecificTags["DDQV"] = SpecificTags["DDQ"];
            SpecificTags["PPHS1"] = pps;
            SpecificTags["PPHS2"] = pps;
            SpecificTags["PPIS1"] = pps;
            SpecificTags["PPIS2"] = pps;
            SpecificTags["VM"] = new ModalVerb();
            SpecificTags["VMK"] = SpecificTags["VM"];

            SpecificWords["as"] = new AS();
            SpecificWords["that"] = new THAT();
            SpecificWords["there"] = new THERE();
            SpecificWords["here"] = SpecificWords["there"];
            SpecificWords["to"] = new TO();
        }
        #endregion

        #region Private Members

        IState NullState = new Null();
	        
        IDictionary<WordClassType, IState> WordClassStates = new Dictionary<WordClassType, IState>()
        {
            {WordClassType.Adposition, new Adposition()},
            {WordClassType.Adjective, new Adjective()},
            {WordClassType.Adverb, new Adverb()},
            {WordClassType.Article, new Article()},
            {WordClassType.Determiner, new Determiner()},
            {WordClassType.Not, new Not()},
            {WordClassType.Noun, new Noun()},
            {WordClassType.Pronoun, new Pronoun()},
            {WordClassType.Verb, new Verb()},
        };
        
        IDictionary<string, IState> SpecificWords = new Dictionary<string, IState>();
        
        IDictionary<string, IState> SpecificTags = new Dictionary<string, IState>();

        #endregion

    }
}
