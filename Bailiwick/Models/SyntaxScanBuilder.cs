using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordIndex = System.Collections.Generic.KeyValuePair<int, Bailiwick.Models.WordInstance>;

namespace Bailiwick.Models
{
    public class SyntaxScanBuilder
    {
        readonly string[] clauseMarkers = { ";", ":", "-", "\u2012", "\u2013", "\u2014", "\u2015" };

        SyntaxScan _scan;
        bool _hasNouns;
        bool _hasVerbs;
        int _currentIndex;

        public SyntaxScanBuilder()
        {
            Reset();
        }

        public SyntaxScanBuilder(SyntaxScanBuilder source)
        {
            _hasVerbs = source._hasVerbs;
            _hasNouns = source._hasNouns;
            _currentIndex = source._currentIndex;

            _scan = new SyntaxScan(source._scan);
        }

        public SyntaxScan Result
        {
            get
            {
                _scan.IsComplete = _hasNouns && _hasVerbs;
                _scan.IsComplex = (_scan.ClausePunctuation + _scan.Commas + _scan.CoordinatingConjunctions + _scan.SubordinatingConjunctions) > 0;
                _scan.Length = _currentIndex + 1;

                return _scan;
            }
        }

        public void Reset()
        {
            _hasNouns = false;
            _hasVerbs = false;
            _currentIndex = 0;

            _scan = new SyntaxScan();
        }

        public void Update(WordInstance w)
        {
            if (_currentIndex == 0)
                _scan.LeadWordClassType = w.GeneralWordClass;

            switch (w.GeneralWordClass)
            {
                case WordClassType.Noun:
                case WordClassType.Pronoun:
                case WordClassType.Determiner:
                    _hasNouns = true;
                    break;

                case WordClassType.Verb:
                    _hasVerbs |= w.PartOfSpeech.Specific != "VM" && w.PartOfSpeech.Specific != "VMK";
                    break;

                case WordClassType.Punctuation:
                    if( w.Instance == ",")
                        _scan.Commas += 1;
                    else if (clauseMarkers.Contains(w.Instance) )
                        _scan.ClausePunctuation += 1;
                    break;

                case WordClassType.Conjunction:
                    if( w.PartOfSpeech.Specific == "CC" || w.PartOfSpeech.Specific == "CCB" )
                        _scan.CoordinatingConjunctions += 1;
                    else
                        _scan.SubordinatingConjunctions += 1;
                    break;
            }

            _currentIndex++;
        }
    }
}
