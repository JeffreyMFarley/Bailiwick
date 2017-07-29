import os
import sys
import json
import csv
import itertools
from itertools import chain
from hew import C45
from gloss import Gloss

def window(seq, n=3):
    "Returns a sliding window (of width n) over data from the iterable"
    "   s -> (s0,s1,...s[n-1]), (s1,s2,...,sn), ...                   "
    " http://stackoverflow.com/questions/6822725/rolling-or-sliding-window-iterator-in-python "
    it = iter(seq)
    result = tuple(itertools.islice(it, n))
    if len(result) == n:
        yield result    
    for elem in it:
        result = result[1:] + (elem,)
        yield result


class ExtractDecisionTreeDataset:
    def __init__(self):
        self.lemmas = {}
        self.pipeline = [self._doesRuleApply]

        self.features = {
                         'A' : lambda w, t: Gloss.isAgent(t),
                         'C' : lambda w, t: Gloss.isClosedWord(t),
                         'Gen' : lambda w, t: t[0],
                         'Ger' : lambda w, t: Gloss.isGerund(t),
                         'Inf' : lambda w, t: Gloss.isInfinite(t),
                         'Ing' : lambda w, t: Gloss.isParticiple(t),
                         'L' : lambda w, t: self._lemma(w),
                         'M' : lambda w, t: Gloss.isModifier(t),
                         'T' : lambda w, t: t,
                         'W' : lambda w, t: w,
                         }

        self.featureSets = [['A', 'C', 'Gen', 'Ger', 'Inf', 'Ing', 'M'],
                            ['L'],
                            ['T'],
                            ['W']]

    #--------------------------------------------------------------------------

    def _lemma(self, w):
        return self.lemmas[w] if w in self.lemmas else '0'

    def _doesRuleApply(self, triplet):
#        return 'w' in triplet[1] and triplet[1]['w'].lower() == 'down'
        return 'w' in triplet[1] and triplet[1]['w'] in self.VJ and triplet[1]['t'] in ['VVN', 'VVG', 'VVD', 'VVO', 'JJ', 'NN1']
#        return (('w' in triplet[1] and triplet[1]['w'].lower() == 'so') or
#                ('mw' in triplet[1] and 'so' in triplet[1]['mw'].lower()))
#        return 'mw' in triplet[1] and "'s" in triplet[1]['mw']
#        return ('t' in triplet[0] and triplet[0]['t'][0] == 'A' and
#                't' in triplet[1] and triplet[1]['t'][0] in ['D'] )
        #return (('w' in triplet[1] and triplet[1]['t'] in ['DA', 'JJ']) or
        #        ('mw' in triplet[1] and triplet[1]['ts'][0] in ['DA', 'JJ']))
        #return (('w' in triplet[1] and triplet[1]['w'].lower() in ['where', 'when']) or
        #        ('mw' in triplet[1] and 'where' in triplet[1]['mw'].lower()) or
        #        ('mw' in triplet[1] and 'when' in triplet[1]['mw'].lower())
        #        )

    #--------------------------------------------------------------------------

    def _maybe(self, s):
        f = filter(self._predicate, s)
        for triplet in window(f):
            for fn in self.pipeline:
                if fn(triplet): 
                    yield triplet

    #--------------------------------------------------------------------------

    def _predicate(self, w0):
        if 'w' not in w0:
            return True
        return w0['t'] != "QD" and w0['t'] != "QQ"

    def _projection(self, triplet):
        w1 = triplet[1]['w'] if 'w' in triplet[1] else triplet[1]['mw']
        t1 = triplet[1]['t'] if 'w' in triplet[1] else triplet[1]['ts'][0]

        w0 = triplet[0]['w'] if 'w' in triplet[0] else triplet[0]['mw']
        t0 = triplet[0]['t'] if 'w' in triplet[0] else triplet[0]['ts'][-1]

        if 'mw' in triplet[1]:
            t2 = triplet[1]['ts'][1]
            w2 = triplet[1]['mw']
        else:
            t2 = triplet[2]['t'] if 'w' in triplet[2] else triplet[2]['ts'][0]
            w2 = triplet[2]['w'] if 'w' in triplet[2] else triplet[2]['mw']

        return {'w0' : w0.lower(), 't0' : t0,
                'w1' : w1.lower(), 't1' : t1,
                'w2' : w2.lower(), 't2' : t2}

    def _projectFeature(self, sextuple, featureCode):
        feature = self.features[featureCode]
        return {featureCode + '-1' : feature(sextuple['w0'], sextuple['t0']),
                featureCode + '+1' : feature(sextuple['w2'], sextuple['t2'])}

    def _buildFeatureSet(self, data, features):
        f0 = []
        for a in data:
            row = {'T' : a['t1']}
            for code in features:
                row.update(self._projectFeature(a, code))
            f0.append(row)
        return f0

    #--------------------------------------------------------------------------

    def run(self, inputFileName, cocaFileName, outputDir):
        # extract
        print('Loading COCA')
        with open(cocaFileName, 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            for row in reader:
                self.lemmas[row['w1'].lower()] = row['L1']
       
        # add some knowns to the lemmas
        self.lemmas['--'] = '-'
        for c in '.?!,:;\'"-':
            self.lemmas[str(c)] = str(c)

        print('Loading Source')
        fullCorpus = {}
        with open(inputFileName, 'r') as f:
            fullCorpus = json.load(f)
        
        print("Loading VJN")
        with open(os.path.join(outputDir, 'word_vjn.txt'), 'r') as f:
            self.VJ = {line.strip() for i, line in enumerate(f)}

        # filter
        print('Filtering')
        a0 = [self._projection(triplet) 
              for k,v in fullCorpus.items() 
              if "?" not in k 
              for triplet in self._maybe(v)]

        # check point save
        print('Saving Set', len(a0), 'rows')
        if a0:
            with open(os.path.join(outputDir, 'sextet.txt'), 'w') as f:
                fieldSet = sorted(a0[0].keys())
                sep = '\t'

                # write the header row
                header = sep.join([col for col in fieldSet])
                f.writelines([header, '\n'])

                # write the rows                
                for row in a0:
                    a = sep.join([str(row[col]) for col in fieldSet])
                    f.writelines([a, '\n'])                

        # build the feature sets
        with open(os.path.join(outputDir, 'decision.txt'), 'w') as f:
            for i, fs in enumerate(self.featureSets):
                features = list(chain.from_iterable([self.featureSets[j-1] 
                                                        for j in range(1,i+2)]))
                f0 = self._buildFeatureSet(a0, features)
                print('Deciding', features)
                C45.makeDecisionTree(f0, 'T', f, 10)


#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

INPUT = '../Tests/Training/BrownCorpusClaws.json'
INPUT_COCA = '../Bailiwick/Corpora/COCA.txt'
OUTPUT = '../data/'

if __name__ == '__main__':
    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    inputFileName = os.path.join(thisScriptDir, INPUT)
    cocaFileName = os.path.join(thisScriptDir, INPUT_COCA)
    outputDir = os.path.join(thisScriptDir, OUTPUT)

    # run the jewels
    process = ExtractDecisionTreeDataset()
    process.run(inputFileName, cocaFileName, outputDir)



    