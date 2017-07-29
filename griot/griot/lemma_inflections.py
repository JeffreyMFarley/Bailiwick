import os
import sys
import json
import csv
import itertools
import griot
from operator import itemgetter
from collections import defaultdict

def _vvdIsOldForm(x):
    a0 = x['L1'] in ['heave', 'bore', 'burn', 'bust', 'dream', 
                     'dwell', 'lean', 'leap', 'learn', 'plead', 
                     'shoe', 'smelt', 'spell', 'spill']
    a1 = x['L1'][-4:] == 'cast'
    a2 = x['w1'][-2:] != 'ed'
    return (a0 or a1) and a2

class LemmaInflections:
    def __init__(self):
        self.lemmas = set()
        self.inflections = defaultdict(dict)
        self.otherVVG = defaultdict(dict)
        self.presPart = griot.Participle('VVG')
        self.pastTense = griot.Participle('VVD')
        self.additions = []
        self.matchedVVG = []

    #--------------------------------------------------------------------------

    def _filterPOS(self, x):
        return x[0:2] in ['VV']

    #--------------------------------------------------------------------------

    def _outputDistribution(self, outputDir):
        self.pos = sorted({y
                           for x in self.inflections 
                           for y in self.inflections[x] 
                           if self._filterPOS(y) })

        print('  Saving Inflections')
        outputDistribution = os.path.join(outputDir, 'lemma_inflections.txt')
        with open(outputDistribution, 'w') as f:
            print('Lemma\t', end='', file=f)
            for pos in self.pos:
                print(pos, end='\t', file=f)
            print('ordered', '1st', '2nd', '3rd', '4th', sep='\t', file=f)

            for lemma in sorted(self.lemmas):
                inflections = self.inflections[lemma]
                print(lemma + '\t', end='', file=f)
                for pos in self.pos:
                    cell = inflections[pos] if pos in inflections else ''
                    print(cell, end='\t', file=f)
                
                sized = [x[0] for x in sorted(inflections.items(), 
                                              key=itemgetter(1), 
                                              reverse=True)]
                print('->'.join(sized), '\t'.join(sized), sep='\t', file=f)

    def _outputParticipleTally(self, outputDir):
        print('  Saving Tally of Present Participle functions')
        output = os.path.join(outputDir, 'participle_pres_tally.txt')
        with open(output, 'w') as f:
            print('Predicate\tCount', file=f)
            for k,v in sorted(self.presPart.tally.items(), 
                              key=itemgetter(1), 
                              reverse=True):
                print(k,v, sep='\t', file=f)

    def _outputUnmatched(self, outputDir):
        print('  Saving Unmatched JJ/NN')

        for match in self.matchedVVG:
            vvg, pos = match['w1'], match['c1']
            if vvg in self.otherVVG[pos]:
                del self.otherVVG[pos][vvg]

        unm = [(vvg, pos, self.otherVVG[pos][vvg]) 
               for pos in self.otherVVG 
               for vvg in self.otherVVG[pos]]
        unm.sort(key=itemgetter(0))
        unm.sort(key=itemgetter(2), reverse=True)
        unm.sort(key=itemgetter(1))

        output = os.path.join(outputDir, 'unmatched_jj_nn.txt')
        with open(output, 'w') as f:
            print('w1\tc1\tcoca\thyphen', file=f)
            for u in unm:
                print(u[0], u[1], str(u[2]), '-' in u[0], sep='\t', file=f)

    #--------------------------------------------------------------------------

    def map(self, x):
        pos, lemma, count = x['c1'], x['L1'], x['coca']
        endsWithIng = x['w1'][-3:] == 'ing'

        if pos in ['VV0', 'VVI']:
            self.lemmas.add(lemma)

        elif pos == 'VVG' and not endsWithIng:
            pos = 'VVG*'

        elif pos == 'VVD' and _vvdIsOldForm(x):
            pos = 'VVDa'

        elif endsWithIng and pos in ['JJ', 'NN1']:
            self.otherVVG[pos][lemma] = count

        if self._filterPOS(pos) and pos not in self.inflections[lemma]:
            self.inflections[lemma][pos] = count

        return x
    
    def __iter__(self):
        for lemma in sorted(self.lemmas):
            vvg = self.presPart.toParticiple(lemma)
            vvd = self.pastTense.toParticiple(lemma)
            for pos in self.otherVVG:
                if vvg in self.otherVVG[pos]:
                    self.inflections[lemma]['VVG/'+pos] = self.otherVVG[pos][vvg]
                    self.matchedVVG.append({'w1': vvg, 'c1': pos})

            if 'VVG' not in self.inflections[lemma]:
                count = int(self.inflections[lemma]['VV0'] * 0.22)
                self.additions.append({'w1': vvg, 'L1': lemma, 
                                       'c1': 'VVG', 'coca': count})
                self.inflections[lemma]['VVG+'] = count

            if 'VVD' not in self.inflections[lemma]:
                count = int(self.inflections[lemma]['VV0'] * 0.22)
                self.additions.append({'w1': vvd, 'L1': lemma, 
                                       'c1': 'VVD', 'coca': count})
                self.inflections[lemma]['VVD+'] = count

        self.index = -1
        return self

    def __next__(self):
        self.index += 1
        if self.index >= len(self.additions):
            raise StopIteration
        return self.additions[self.index]

    def __str__(self):
        return 'Lemma Inflections'

#------------------------------------------------------------------------------
# Main
#------------------------------------------------------------------------------

if __name__ == '__main__':
    singleton = LemmaInflections()

    # process
    result = list(map(singleton.map, griot.iterateSource()))
    griot.save(list(singleton))

    for fn in [
               singleton._outputDistribution, 
               #singleton._outputParticipleTally,
               #singleton._outputUnmatched,
               ]:
        fn('../data/')



    
