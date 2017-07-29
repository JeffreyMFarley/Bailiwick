import unittest
import os
import sys
import griot
from operator import itemgetter

def _vvgFilter(x):
    return x['w1'][-2:] != 'in'

def _vvdFilter(x):
    a0 = x['L1'] in ['heave', 'bore', 'burn', 'bus', 'bust', 
                     'dream', 'dwell', 'lean', 'leap', 'learn', 'plead', 
                     'shoe', 'smelt', 'spell', 'spill']
    a1 = x['L1'][-4:] == 'cast'
    a2 = x['w1'][-2:] != 'ed'
    return not((a0 or a1) and a2)

class Test_Participle(unittest.TestCase):
    def _trainingSet(self, type, secondaryFilter):
        known = {}
        for x in filter(secondaryFilter, filter(griot.keep, griot.iterateSource())):
            if x['c1'] == type and x['L1'] not in known:
                known[x['L1']] = x
        
        errors = []

        target = griot.Participle(type)
        for lemma in sorted(known):
            expected, count = known[lemma]['w1'], known[lemma]['coca'] 
            actual = target.toParticiple(lemma)
            if actual != expected:
                errors.append({'w1': actual, 'w[-2:]': actual[-2:], 'w[-3:]': actual[-3:], 
                               'L1': lemma, 'l[-2:]': lemma[-2:], 'l[-3:]': lemma[-3:],
                               'expected': expected, 
                               'coca': count})

        fieldSet = ['expected', 'w1', 'w[-2:]', 'w[-3:]', 'L1', 'l[-2:]', 'l[-3:]', 'coca']
        errors.sort(key=itemgetter('l[-3:]'))
        errors.sort(key=itemgetter('L1'))
        errors.sort(key=itemgetter('l[-2:]'))

        with open('../data/'+type+'_errors.txt', 'w') as f:
            print('\t'.join(fieldSet), file=f)
            for e in errors:
                print('\t'.join([str(e[x]) for x in fieldSet]), file=f)

        return len(errors)

    def testVVG(self):
        self.assertEqual(2, self._trainingSet('VVG', _vvgFilter))

    def testVVD(self):
        self.assertEqual(6, self._trainingSet('VVD', _vvdFilter))

if __name__ == '__main__':
    unittest.main()
