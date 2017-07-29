using Bailiwick.Models;
using Bailiwick.Models.Phrases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bailiwick.Analysis
{
    delegate void PostPhraseProcess(Sentence s);

    public class PostPhraseBuilder : ISentenceOperation
    {
        public PostPhraseBuilder(IConvert tryConvert, IConvert forceConvert)
        {
            TryConvert = tryConvert;
            ForceConvert = forceConvert;

            //DebugDump = true;
        }

        IConvert TryConvert { get; set; }
        IConvert ForceConvert { get; set; }
        bool DebugDump { get; set; }

        public void Process(Sentence s)
        {
            var pipeline = new PostPhraseProcess[] 
            {
                ResolveAmibiguous,
                FindOrphans
            };

            foreach (var op in pipeline)
                op(s);
    
        }

        #region Orphan Process

        void FindOrphans(Sentence s)
        {
            var orphanage = new List<WordInstance>();

            for (int i = 0; i < s.Length; i++)
            {
                var w = s.Words[i];
                var currentIsMarker = w.GeneralWordClass == WordClassType.Punctuation ||
                                      w.GeneralWordClass == WordClassType.Conjunction;

                var segments = s[i].ToArray();

                // Found an orphan
                if (segments.Length == 1 && !currentIsMarker)
                    orphanage.Add(w);

                // Handle orphans
                else if (orphanage.Count > 0)
                {
                    if (currentIsMarker)
                        MakeNewPhrase(s, w, orphanage);

                    else
                        HandleOrphans(s, segments, orphanage);

                    orphanage.Clear();
                }
            }
        }

        void MakeNewPhrase(Sentence s, WordInstance currentWord, IList<WordInstance> orphanage)
        {
            var action = "Make New Phrase";

            // Subordinating Phrase
            if (currentWord.IsSubordinatingConjunction())
            {
                var csp = new SubordinatingPhrase(orphanage, currentWord);
                s.Add(csp);

                Dump(s, orphanage, currentWord, "x - Subordinating Phrase");
            }

            // Adverbs
            else if (orphanage.All(x => x.GeneralWordClass == WordClassType.Adverb))
            {
                action = "x - RPhrase";

                // Normal adverb phrase
                if (currentWord.IsEndOfSentence() || currentWord.IsCoordinatingConjunction())
                {
                    var rp = new AdverbPhrase(orphanage);
                    s.Add(rp);
                }

                // Adverb phrase plus adverbial comma
                else if (currentWord.PartOfSpeech.Specific == "QC")
                {
                    currentWord.PartOfSpeech = WordClasses.Specifics["QCR"];
                    var rp = new AdverbPhrase(orphanage);
                    s.Add(rp);
                }

                else
                    action = "RPhrase";

                Dump(s, orphanage, currentWord, action);
            }

            // OMG! Ending a clause with a preposition, tsk, tsk
            else if (currentWord.GeneralWordClass == WordClassType.Punctuation && orphanage.All(x => x.GeneralWordClass == WordClassType.Adposition))
                NewEllipsisNounPhrase(s, currentWord, orphanage);

            else
                Dump(s, orphanage, currentWord, action);
        }

        void HandleOrphans(Sentence s, ISentenceNode[] segments, IList<WordInstance> orphanage)
        {
            Debug.Assert(orphanage.Count > 0);

            // Found a home?
            var foster = segments[0] as IPhrase;
            Debug.Assert(foster != null);

            // Add adverbs to the front of the next phrase
            if ( orphanage.All(x => foster.IsSupported(x) && x.GeneralWordClass == WordClassType.Adverb) )
            {
                Dump(s, orphanage, foster, "x - RPhrase");

                var rp = new AdverbPhrase(orphanage);
                s.Add(rp);
 
                return;
            }

            // Ellipse Noun found
            if (foster.Head.IsAdjectival())
            {
                NewEllipsisNounPhrase(s, foster, orphanage);
                return;
            }

            // Verb that should be an adjective or noun found
            if (foster.Head.GeneralWordClass == WordClassType.Verb && foster.ClassColocates.Count == 1)
            {
                var action = "";

                var list = foster.ToArray();
                foreach (var w in list.Where(x => !x.IsNounPhrasePart()))
                {
                    if( action.Length != 0 )
                        action += ";";
                    action += "Converting " + w.Normalized + " ";

                    if (!TryConvert.ToNounPhrasePart(w)) 
                    {
                        action += "[Forced]";
                        ForceConvert.ToNounPhrasePart(w);
                    }
                }

                // Is there an adjacent phrase?
                var neighbor = s.Next(foster).FirstOrDefault();

                // Put everything on the neighbor
                if (neighbor != null)
                {
                    action += "; pushed on " + neighbor.Head.GeneralWordClass;
                    Dump(s, orphanage, foster, action);
                    return;
                }

                // Convert to a noun ellipsis phrase
                action += "; Ellipsis Noun?";
                Dump(s, orphanage, foster, action);
                return;
            }

            // Verb-like orphans that should be converted to noun phrase roles
            if( orphanage.All(x => x.IsVerbPhrasePart()) )
            {
                var action = "";

                var list = foster.ToArray();
                foreach (var w in list.Where(x => !x.IsNounPhrasePart()))
                {
                    if( action.Length != 0 )
                        action += ";";
                    action += "Converting " + w.Normalized + " ";

                    if (!TryConvert.ToNounPhrasePart(w)) 
                    {
                        action += "[Forced]";
                        ForceConvert.ToNounPhrasePart(w);
                    }
                }

                return;
            }

            Dump(s, orphanage, foster, "No matching branch");
        }

        void NewEllipsisNounPhrase(Sentence s, IPhrase foster, IList<WordInstance> orphanage)
        {
            // Does this have to be an ellipsis?
            var clone = new WordInstance(foster.Head.Instance, foster.Head);
            if (TryConvert.ToAgent(clone))
            {
                Dump(s, orphanage, foster, "Can Convert");
                return;
            }

            Dump(s, orphanage, foster, "x - Ellipsis Noun");

            s.Remove(foster);
            var enp = new EllipsisNounPhrase(orphanage, foster);
            s.Add(enp);
        }

        void NewEllipsisNounPhrase(Sentence s, ISentenceNode w, IList<WordInstance> orphanage)
        {
            Dump(s, orphanage, w, "x - Ellipsis Noun");

            var enp = new EllipsisNounPhrase(orphanage, null);
            s.Add(enp);
        }

        void Dump(Sentence s, IList<WordInstance> list, ISentenceNode w, string note)
        {
            if (!DebugDump)
                return;

            var start = Math.Min(w.StartIndex, list.Min(x => x.StartIndex)) - 2;
            var end = Math.Max(w.EndIndex, list.Max(x => x.EndIndex)) + 2;

            Console.WriteLine(string.Join("\t"
                                            , note
                                            , string.Join(" ", list.Select(x => x.Instance))
                                            , string.Join(", ", list.Select(x => x.PartOfSpeech.General))
                                            , string.Join(", ", list.Select(x => x.PartOfSpeech.Specific))
                                            , w.Head.Instance
                                            , w.Head.PartOfSpeech.General
                                            , w.Head.PartOfSpeech.Specific
//                                                  , string.Join(" ", s.Words.Where(x => x.StartIndex >= start && x.EndIndex <= end).Select(x => x.Instance))
                                            , s.ID.ToString()
                                            , string.Join(" ", s.Words.Where(x => x.PartOfSpeech.Specific != "QEN").Select(x => x.Instance))
                                            ));
        }

        #endregion

        void ResolveAmibiguous(Sentence s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                // Check the number of phrases at this location
                var segments = s[i].ToArray();
                if( segments.Length < 3 )
                    continue;

                // More than expected
                if (segments.Length > 3)
                {
                    Console.WriteLine("Unexpected number of ambiguities in ResolveAmbiguous {0}", segments.Length);
                    continue;
                }

                var prev = segments[0] as IPhrase;
                var next = segments[1] as IPhrase;
                var word = segments[2] as WordInstance;

                // The sequence is not expected
                if (prev == null || next == null || word == null)
                {
                    Console.WriteLine("Unexpected sequence in ResolveAmbiguous {0} {1} {2}", segments[0], segments[1], segments[2]);
                    continue;
                }

                // Ties goes to the noun
                if (prev.GeneralWordClass == WordClassType.Adjective && next.GeneralWordClass == WordClassType.Noun)
                {
                    s.Remove(prev);
                    continue;
                }
                if (prev.GeneralWordClass == WordClassType.Noun && next.GeneralWordClass == WordClassType.Adjective)
                {
                    s.Remove(next);
                    continue;
                }

                // Ties go to the verb
                if (prev.GeneralWordClass == WordClassType.Verb && next.GeneralWordClass == WordClassType.Adverb)
                {
                    s.Remove(next);
                    continue;
                }
                if (next.GeneralWordClass == WordClassType.Verb && prev.GeneralWordClass == WordClassType.Adverb)
                {
                    s.Remove(prev);
                    continue;
                }


                // By default, the ambiguous section will belong to the next phrase, not the previous
                var loser = prev;

                // Handle Special cases
                if (prev.GeneralWordClass == WordClassType.Verb && next.GeneralWordClass == WordClassType.Noun && word.GeneralWordClass == WordClassType.Adverb)
                {
                    var immedateNext = next.AdjunctColocates.SkipWhile(x => x.GeneralWordClass == WordClassType.Adverb).FirstOrDefault();
                    if (immedateNext == null || immedateNext.GeneralWordClass != WordClassType.Adjective)
                        loser = next;
                }

                else if (next.GeneralWordClass == WordClassType.Verb && prev.GeneralWordClass == WordClassType.Noun && word.PartOfSpeech.Specific == "RA")
                {
                    loser = next;
                }

                // Are there any other words in the ambiguous section?
                int extra = prev.EndIndex - next.StartIndex;

                // Transfer ownership
                if( extra == 0 )
                    s.Trim(loser, word);
                else
                    s.Trim(loser, i, i + extra);

                Debug.Assert(s[i].Count() == 2, string.Join(" ", s.Words.Where(x => x.StartIndex >= (i - 3) && x.EndIndex <= (i + 3)).Select(x => x.Instance)));
            }
        }
    }   
}
