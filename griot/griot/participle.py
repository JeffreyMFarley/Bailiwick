import csv
from collections import Counter

class Participle:
    def __init__(self, type):
        self.tally = Counter()
        self.vowels = [c for c in 'aeiou']
        self.doublingConsonants = [c for c in 'bdgklmnprstvz']

        self.type = type
        self.ending = 'ing' if type == 'VVG' else 'ed'

        self.exceptions = self._loadExceptions()
        self.pipeline = self._loadPipeline()

    #--------------------------------------------------------------------------

    def _addD(self, w0):
        if len(w0) > 1 and w0[-1] == 'e' and w0[-2] in ['e', 'o', 'y']:
            return w0 + 'd'

    def _addK(self, w0):
        if w0[-2:] in ['ac', 'ic']:
            return w0 + 'k' + self.ending

    def _default(self, w0):
        return w0 + self.ending

    def _doubleConsonant(self, w0):
        if len(w0) > 1 and w0[-2] in self.vowels and w0[-1] in self.doublingConsonants:
            root = w0[:-1]
            double = w0[-1]
            return root + double + double + self.ending

    def _doubleVowel(self, w0):
        if len(w0) > 2 and w0[-3] in self.vowels and w0[-2] in self.vowels:
            return w0 + self.ending

    def _dropE(self, w0):
        if len(w0) > 1 and w0[-1] == 'e' and w0[-2] not in ['e', 'o', 'y']:
            return w0[:-1] + self.ending

    def _dropIEaddY(self, w0):
        if w0[-2:] == 'ie':
            return w0[:-2] + 'y' + self.ending

    def _dropYaddIED(self, w0):
        if len(w0) > 1 and w0[-1] == 'y' and w0[-2] not in ['a', 'e', 'o']:
            return w0[:-1] + 'ied'

    def _fer(self, w0):
        if w0[-3:] == 'fer' and w0[-4:] != 'ffer':
            return w0 + 'r' + self.ending

    def _fit(self, w0):
        if w0[-3:] != 'fit':
            return None

        if w0 in ['benefit', 'discomfit', 'profit']:
            return w0 + self.ending
        else:
            root = w0[:-1]
            double = w0[-1]
            return root + double + double + self.ending

    def _noDoublingCheckLast2(self, w0):
        ''' 
        check for length greater than 3 because 'pen' and 'don' double, 
        but not 'open' and 'abandon'
        '''
        if len(w0) > 3 and w0[-2:] in ['al', 'er', 'es', 'el', 'en', 'om', 
                                       'on', 'or', 'us']:
            return w0 + self.ending

    def _noDoublingCheckLast3(self, w0):
        ''' 
        check for length greater than 3 because 'pet' and 'vet' double, 
        but not 'carpet' and 'rivet'
        '''
        if len(w0) > 3 and w0[-3:] in ['bit', 'cil', 'cit', 'dit', 'gar', 
                                       'het', 'ket', 'lar', 'met', 'pet', 
                                       'ret', 'ril', 'rit', 'sit', 'vet', 
                                       'vil', 'xit']:
            return w0 + self.ending

    def _noDoublingCheckLast4(self, w0):
        if w0[-4:] in ['dget', 'elop', 'llop']:
            return w0 + self.ending

    def _noDoublingCheckLast5(self, w0):
        if w0[-5:] in ['carol', 'debut', 'kayak', 'limit', 'pilot', 'pivot', 'rival', 'vomit']:
            return w0 + self.ending

    def _noDoublingCheckLast6(self, w0):
        if w0[-6:] in ['ballot', 'buffet', 'combat', 'closet', 'corset', 'cosset', 'fillet', 'gambol', 'gossip', 'mortar', 'murmur', 'parrot', 'summit', 'target']:
            return w0 + self.ending

    def _noDoublingCheckLast7(self, w0):
        if w0[-7:] in ['bayonet', 'catalog', 'chagrin']:
            return w0 + self.ending

    def _pel(self, w0):
        if w0[-3:] == 'pel':
            return w0 + 'l' + self.ending

    def _qDoubleVowel(self, w0):
        ''' squat, acquit, quiz, quit, equip
            but not equal and conquer
        '''
        if w0[-4:] in ['qual', 'quer']:
            return None

        if (len(w0) > 3 and w0[-4] == 'q' 
            and w0[-3] in self.vowels and w0[-2] in self.vowels
            and w0[-1] in self.doublingConsonants):
            root = w0[:-1]
            double = w0[-1]
            return root + double + double + self.ending

    #--------------------------------------------------------------------------

    def _loadExceptions(self):
        with open('../data/participle_exceptions.txt', 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            return {x['VV0'].strip(): x[self.type].strip() for x in reader}

    def _exceptions(self, w0):
        if w0 in self.exceptions:
            return self.exceptions[w0]

    #--------------------------------------------------------------------------

    def _loadPipeline(self):
        if self.type == 'VVG':
            return [self._exceptions,
                    self._dropIEaddY, 
                    self._dropE, # Most common rule
                    self._addK,
                    self._qDoubleVowel,
                    self._doubleVowel, # 3rd most common rule
                    self._fer,
                    self._fit,
                    self._pel,
                    self._noDoublingCheckLast2,
                    self._noDoublingCheckLast3,
                    self._noDoublingCheckLast4,
                    self._noDoublingCheckLast5,
                    self._noDoublingCheckLast6,
                    self._noDoublingCheckLast7,
                    self._doubleConsonant, # 4th most common rule
                    self._default] # 2nd most common rule
        elif self.type == 'VVD':
            return [self._exceptions,
                    self._addD,
                    self._dropE,
                    self._addK,
                    self._dropYaddIED,
                    self._qDoubleVowel,
                    self._doubleVowel,
                    self._fer,
                    self._fit,
                    self._pel,
                    self._noDoublingCheckLast2,
                    self._noDoublingCheckLast3,
                    self._noDoublingCheckLast4,
                    self._noDoublingCheckLast5,
                    self._noDoublingCheckLast6,
                    self._noDoublingCheckLast7,
                    self._doubleConsonant,
                    self._default]
        return []

    #--------------------------------------------------------------------------

    def toParticiple(self, w0):
        for fn in self.pipeline:
            participle = fn(w0)
            if participle:
                self.tally[fn.__name__] += 1
                return participle
