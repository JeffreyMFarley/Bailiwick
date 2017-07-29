import os
import sys
import re
import json
from collections import defaultdict
import xml.etree.ElementTree as ET
import itertools
import operator

NS0 = 'http://www.w3.org/XML/1998/namespace'
NS1 = 'http://www.tei-c.org/ns/1.0'

E_TEXT = '{'+NS1+'}text'
E_SENTENCE = '{'+NS1+'}s'
E_WORD = '{'+NS1+'}w'
E_PUNCT = '{'+NS1+'}c'
E_MULTIWORD = '{'+NS1+'}mw'

A_ID = '{'+NS0+'}id'
A_SET = 'decls'
A_NUMBER = 'n'
A_BROWNTAG = 'type'
A_BROWNMULTITAG = 'pos'

def outputProjection(x):
    return {k:x[k] for k in ['t', 'ts', 'mw', 'w'] if k in x}

class ConvertBrownCorpus:
    def __init__(self):
        self.simpleTag = self._loadSimpleTranslationTable()
        self.complexTag = self._loadComplexTranslationTable()
        self.punctuationTag = self._loadPunctuationTable()
        self.foreignWords = self._loadForeignWords()

        self.lastBrownWord = ''

        self.pipeline = [self._handleForeignWord,
                         self._handleEVERY,
                         self._handleTHATasPronoun,
                         self._handleMultipleWord,
                         self._handleReallyAdjectives,
                         self._handleSimpleApostropheS,
                         self._handleSimpleTag,
                         self._handleComplexTag,
                         self._handlePunctuationTag,
                         self._unhandled]

        self.splitHyphenTest = re.compile('^\d+-[a-zA-Z]')
        self.numberTest = re.compile('\d+')

        self.errorLog = []
    
   #---------------------------------------------------------------------------

    def _extract(self, inputFileName):
        tree = ET.parse(inputFileName)
        textGen = (x for x in tree.iter(E_TEXT)) 
        for t in textGen:
            sentenceGen = (x for x in t.iter(E_SENTENCE))
            for s in sentenceGen:
                k = self._extractKey(t,s)
                wordGen = (x for x in s.iter() if x.tag != E_SENTENCE)
                for i, w in enumerate(wordGen):
                    yield self._extractValue(w, k, i)

    def _extractKey(self, t, s):
        return t.attrib[A_ID] + '-' + "{0:03d}".format(int(s.attrib[A_NUMBER]))

    def _extractValue(self, w, k, i):
        if w.tag != E_MULTIWORD:
            return {'k' : k, 'i' : i, 'w':w.text, 'bt':w.attrib[A_BROWNTAG]} 
        return {'k' : k, 'i' : i, 
                'mw':w.text.strip(), 
                'bts':self._convertMultiTag(w)}

    def _convertMultiTag(self, w):
        return w.attrib[A_BROWNMULTITAG].split(' ')

    #--------------------------------------------------------------------------

    def _loadSimpleTranslationTable(self):
        return {
        'NEG': 'XX',
        '*' : 'XX',
        'NIL': 'FU',
        'ABL': 'RR',
        'ABN': 'DB',
        'ABX': 'DB2',
        'AP': 'DA',
        'AT': 'AT',
        'BE': 'VB0',
        'BED': 'VBDR',
        'BEDZ': 'VBDZ',
        'BEG': 'VBG',
        'BEM': 'VBM',
        'BEN': 'VBN',
        'BER': 'VBR',
        'BEZ': 'VBZ',
        'CC': 'CC',
        'CD': 'MC',
        'CS': 'CS',
        'DO': 'VD0',
        'DOD': 'VDD',
        'DOZ': 'VDZ',
        'DT': 'DD1',
        'DTI': 'DD',
        'DTS': 'DD2',
        'DTX': 'DB',
        'EX': 'EX',
        'HV': 'VH0',
        'HVD': 'VHD',
        'HVG': 'VHG',
        'HVN': 'VHN',
        'HVZ': 'VHZ',
        'IN': 'II',
        'JJ': 'JJ',
        'JJR': 'JJR',
        'JJS': 'JJT',
        'JJT': 'JJT',
        'MD': 'VM',
        'NN': 'NN1',
        'NNS': 'NN2',
        'NP': 'NP1',
        'NPS': 'NP2',
        'NR': 'NN',
        'NRS': 'NN2',
        'OD': 'MD',
        'PN': 'PN1',
        'PPg': 'APPGE',
        'PPgg': 'PPGE',
        'PPL': 'PPX1',
        'PPLS': 'PPX2',
        'PPO': 'PPHO1',
        'PPS': 'PPHS1',
        'PPSS': 'PPHS2',
        'QL': 'RG',
        'QLP': 'RR',
        'RB': 'RR',
        'RBR': 'RGR',
        'RBT': 'RGT',
        'RN': 'RR',
        'RP': 'RP',
        'TO': 'TO',
        'UH': 'UH',
        'VB': 'VV0',
        'VBD': 'VVD',
        'VBG': 'VVG',
        'VBN': 'VVN',
        'VBZ': 'VVZ',
        'WDT': 'DDQ',
        'WPg': 'DDQGE',
        'WPO': 'PNQO',
        'WPS': 'PNQS',
        'WQL': 'DDQ',
        'WRB': 'RRQ'}

    def _loadComplexTranslationTable(self):
        return {
        'APg': ['DA','GE'],
        'CDg': ['MC', 'GE'],
        'DTg': ['DD1', 'GE'],
        'JJg': ['JJ', 'GE'],
        'NNg': ['NN1', 'GE'],
        'NNSg': ['NN2', 'GE'],
        'NPg': ['NP1', 'GE'],
        'NPSg': ['NP2', 'GE'],
        'NRg': ['NN', 'GE'],
        'PNg': ['PN1', 'GE'],
        'RBg': ['RR', 'GE']}

    def _loadPunctuationTable(self):
        general = {x : 'QQ' for x in ':(){}[]+/'}
        general.update({
                ',':'QC',
                '\'': 'QSQ',
                '"' : 'QD',
                '``' : 'QD',
                '\'\'' : 'QD',
                '!' : 'QEE',
                '.' : 'QEP',
                '?' : 'QEQ',
                '\'?\'' : 'QEQ',
                '-' : 'QH',
                '--' : 'QH',
                ';' : 'QS'})
        return general

    def _loadForeignWords(self):
        thisScriptDir = os.path.dirname(__file__)
        fileName = os.path.join(thisScriptDir, 'BrownForeignWords.txt')
        with open(fileName, 'r') as f:
            return set([line.strip() for line in f])

    #--------------------------------------------------------------------------

    def _handleForeignWord(self, bw):
        word = bw['w'] if 'w' in bw else bw['mw']
        if word.lower() in self.foreignWords:
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': word, 
                    't' : 'FW' }

    def _handleEVERY(self, bw):
        if 'w' in bw and bw['w'].lower() == 'every':
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': bw['w'], 
                    't' : 'DD1' }

    def _handleTHATasPronoun(self, bw):
        if ('w' in bw 
            and bw['w'].lower() == 'that' 
            and bw['bt'] in ['WPO', 'WPS']):
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': bw['w'], 
                    't' : 'DD1' }
        if ('mw' in bw 
            and 'that' in bw['mw'].lower() 
            and bw['bts'][0] in ['WPO', 'WPS']):
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'mw': bw['mw'], 
                    'ts' : ['DD1', self.simpleTag[bw['bts'][1]]] }

    def _handleMultipleWord(self, bw):
        if 'mw' in bw:
            ts = []
            for t0 in bw['bts']:
                if t0 not in self.simpleTag:
                    return None
                else:
                    ts.append(self.simpleTag[t0])
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'mw': bw['mw'], 
                    'ts' : ts }

    def _handleReallyAdjectives(self, bw):
        if 'w' in bw and bw['w'].lower() in ['long-term']:
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': bw['w'], 
                    't' : 'JJ' }

    def _handleSimpleApostropheS(self, bw):
        if ('w' in bw and bw['bt'] in self.simpleTag and len(bw['w']) > 3
            and "'s" == bw['w'][-2:]):
            words = bw['w'].split("'")
            if self.numberTest.search(words[0]):
                if len(words[0]) == 4:
                    return {'k': bw['k'], 
                            'i': bw['i'], 
                            'w': bw['w'], 
                            't' : "NNT1"}
                else:
                    return {'k': bw['k'], 
                            'i': bw['i'], 
                            'w': bw['w'], 
                            't' : "MC"}
            else:
                return {'k': bw['k'], 
                        'i': bw['i'], 
                        'mw': bw['w'], 
                        'ts' : [self.simpleTag[bw['bt']], "GE"]}

    def _handleSimpleTag(self, bw):
        if 'w' in bw and bw['bt'] in self.simpleTag:
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': bw['w'], 
                    't' : self.simpleTag[bw['bt']] }

    def _handleComplexTag(self, bw):
        if 'w' in bw and bw['bt'] in self.complexTag:
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'mw': bw['w'], 
                    'ts' : self.complexTag[bw['bt']] }

    def _handlePunctuationTag(self, bw):
        if 'w' in bw and bw['w'] in self.punctuationTag:
            return {'k': bw['k'], 
                    'i': bw['i'], 
                    'w': bw['w'], 
                    't' : self.punctuationTag[bw['w']] }

    def _unhandled(self, bw):
        self.errorLog.append('Unmatched: ' + json.dumps(bw))

    #--------------------------------------------------------------------------

    def _isInterrogative(self, cws):
        q = [x for x in cws if 't' in x and x['t'] == 'QEQ']
        return len(q) > 0

    #--------------------------------------------------------------------------

    def _noDoublePunct(self, bw):
        if 'mw' in bw:
            return True

        if bw['w'] in ['!', '?'] and bw['w'] == self.lastBrownWord:
            self.lastBrownWord = bw['w']
            return False

        self.lastBrownWord = bw['w']
        return True

    def _simpleConvert(self, bw):
        for fn in self.pipeline:
            cw = fn(bw)
            if cw: 
                return cw

    #--------------------------------------------------------------------------

    def _breakHyphens(self, wordGen):
        for w in wordGen:
            if 'w' in w and self.splitHyphenTest.search(w['w']):
                for i, token in enumerate(w['w'].split('-')):
                    if i > 0:
                        yield {'k': w['k'],
                               'i': w['i'], 
                               'w': '-', 
                               't' : 'QH' }
                    yield {'k': w['k'], 
                           'i': w['i'], 
                           'w': token, 
                           't' : 'MC' if self.numberTest.search(token) else 'N' }
            else:
                yield w

    #--------------------------------------------------------------------------


    def run(self, inputFileName, outputFileName):
        # extract
        print('Running')
        gen0 = self._extract(inputFileName)
        gen1 = filter(self._noDoublePunct, gen0)
        gen2 = map(self._simpleConvert, gen1)
        gen3 = self._breakHyphens(gen2)

        store = {k:[outputProjection(x) for x in g]
                 for k,g in itertools.groupby(gen3, operator.itemgetter('k'))}
        
        # load
        print('Saving Results')
        with open(outputFileName, 'w') as f:
            print('{', file=f)
            for k in sorted(store):
                keyFormat = ('"{0}" : ' if not self._isInterrogative(store[k]) 
                             else '"{0}?" : ')
                print(keyFormat.format(k), file=f, end='')
                json.dump(store[k], f, sort_keys=True)
                if k != 'R09-099' :
                    print(',', file=f)
            print('}', file=f)

#------------------------------------------------------------------------------
# Main
#------------------------------------------------------------------------------

INPUT = '../Test Corpus/BrownCorpus.xml'
OUTPUT = '../Tests/Training/BrownCorpusClaws.json'

if __name__ == '__main__':
    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    inputFileName = os.path.join(thisScriptDir, INPUT)
    outputFileName = os.path.join(thisScriptDir, OUTPUT)

    # convert
    converter = ConvertBrownCorpus()
    converter.run(inputFileName, outputFileName)

    fileName = os.path.join(thisScriptDir, '../data/brown_convert_error.txt')
    with open(fileName, 'w') as f:
        for e in converter.errorLog:
            print(e, file=f)


    