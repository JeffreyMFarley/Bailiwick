import os
import sys
import re
import json
import csv
import itertools
from collections import defaultdict, Counter
from operator import itemgetter, attrgetter
from gloss import Gloss

class BrownCocaGap:
    def __init__(self):
        self.coca = defaultdict(dict)
        self.brown = defaultdict(Counter)

        self.bmw = defaultdict(Counter)

        self.pcw = set()   # pcw = PartitionCommonWords
        self.pcwB1C1 = {} # brown one, coca one
        self.pcwB1CN = {} # brown one, coca many
        self.pcwBNC1 = {} # brown many, coca one
        self.pcwBNCN = {} # brown many, coca many

        self.pbo = set()   # pbo = PartitionBrownOnly
        self.pboClosed = []
        self.pboOpen = []

        self.numberTest = re.compile('\d')
        self.acronymTest = re.compile('[a-zA-Z]\.[a-zA-Z]')

    #-------------------------------------------------------------------------------

    def _isValidWord(self, word, tag):
        return (len(word) > 2 and tag not in ['FU', 'FW', 'NP1', 'NP2']
                and not self.numberTest.search(word) 
                and not self.acronymTest.search(word));

    def _processMultiTag(self, word, tags):
        if (word[-1] == '\'' 
            or word[-2:] in ['\'s', '\'d', '\'m'] 
            or word[-3:] in ['n\'t', '\'re', '\'ll', '\'ve']):
            return

        ignore = set(['FW', 'FU', 'NP1', 'NP2'])
        result = set(tags) - ignore
        if len(result) < 2:
            return

        self.bmw[word][' '.join(tags)] += 1

    def _brownOnlySubPartitions(self):
        for w in sorted(self.pbo):
            for k,v in self.brown[w].items():
                if Gloss.isClosedWord(k):
                    self.pboClosed.append({'w1': w, 'c1' : k, 'coca': v})
                elif v > 1 and '.' not in w and '\'' not in w:
                    self.pboOpen.append({'w1': w, 'c1' : k, 'coca': v})

    def _commonWordSubPartitions(self):
        for w in sorted(self.pcw):
            brownSet = set([x.upper() for x in self.brown[w] if self.brown[w][x] > 1 ])
            cocaSet = set([x.upper() for x in self.coca[w]])
            brownDelta = list(brownSet - cocaSet)
            cocaDelta = list(cocaSet - brownSet)
            
            bgs = set([x[0] for x in brownDelta])
            cgs = set([x[0] for x in cocaDelta])
            generalDelta = bgs - cgs
            generalSame = bgs & cgs
            if not len(generalSame):  # easy case, no general or specific overlap
                self._routeToSubPartition(w, brownDelta, cocaDelta)
            elif len(generalDelta) < len(bgs):  # remove the general case overlaps
                for r in [x for x in brownDelta if x[0] in generalSame]:
                    brownDelta.remove(r)
                for r in [x for x in cocaDelta if x[0] in generalSame]:
                    cocaDelta.remove(r)
                self._routeToSubPartition(w, brownDelta, cocaDelta)

    def _routeToSubPartition(self, w, bd, cd):
        if len(bd) == 1 and len(cd) <= 1:
            self.pcwB1C1.update({w : {'bd': bd, 'cd' : cd}})
        elif len(bd) > 1 and len(cd) <= 1:
            self.pcwBNC1.update({w : {'bd': bd, 'cd' : cd}})
        elif len(bd) == 1 and len(cd) > 1:
            self.pcwB1CN.update({w : {'bd': bd, 'cd' : cd}})
        elif len(bd) > 1 and len(cd) > 1:
            self.pcwBNCN.update({w : {'bd': bd, 'cd' : cd}})

    def _generateOutputRecords(self):
        for s in [pipeline.pcwB1C1, pipeline.pcwB1CN, pipeline.pcwBNC1, pipeline.pcwBNCN]:
            for w in s:
                for t in s[w]['bd']:
                    yield {'w1': w, 'c1' : t, 'coca': self.brown[w][t]}

    #-------------------------------------------------------------------------------

    def run(self, brownFileName, cocaFileName, outputDir):
        # extract
        print('Loading COCA')
        with open(cocaFileName, 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            for row in reader:
                self.coca[row['w1'].lower()][row['c1']] = int(row['coca'])
        
        print('Loading Brown')
        with open(brownFileName, 'r') as f:
            suite = json.load(f)
            chain = itertools.chain.from_iterable(suite.values())
            for tag in chain:
                if 'w' in tag and self._isValidWord(tag['w'], tag['t']):
                    self.brown[tag['w'].lower()][tag['t']] += 1
                elif 'mw' in tag:
                    self._processMultiTag(tag['mw'].lower(), tag['ts'])

        # initial partitions
        self.pcw = set(self.coca) & set(self.brown)
        self.pbo = set(self.brown) - set(self.coca)

        # partition 1
        self._commonWordSubPartitions()
        
        # partition 2
        self._brownOnlySubPartitions()

        # load
        print('Saving Results')
        with open(os.path.join(outputDir, 'brown_only_open.txt'), 'w') as f:
            for d in sorted(pipeline.pboOpen, key=itemgetter('c1', 'w1')):
                print(d['w1'] + '\t' + d['c1'], file=f)

        with open(os.path.join(outputDir, 'brown_only_closed.txt'), 'w') as f:
            for d in sorted(pipeline.pboClosed, key=itemgetter('c1', 'w1')):
                print(d['w1'] + '\t' + d['c1'], file=f)

        with open(os.path.join(outputDir, 'brown_adds.txt'), 'w') as f:
            for d in sorted(self._generateOutputRecords(), key=itemgetter('c1', 'w1')):
                print(d['w1'] + '\t' + d['c1'] + '\t' + str(d['coca']), file=f)

#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

INPUT_COCA = '../Bailiwick/Corpora/COCA.txt'
INPUT_BROWN = '../Tests/Training/BrownCorpusClaws.json'
OUTPUT = '../data/'

if __name__ == '__main__':
    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    brownFileName = os.path.join(thisScriptDir, INPUT_BROWN)
    cocaFileName = os.path.join(thisScriptDir, INPUT_COCA)
    outputDir = os.path.join(thisScriptDir, OUTPUT)

    # run the jewels
    pipeline = BrownCocaGap()
    pipeline.run(brownFileName, cocaFileName, outputDir)

    print('COCA', len(pipeline.coca), 'Brown', len(pipeline.brown))

    #for s in [pipeline.pcwB1C1, pipeline.pcwB1CN, pipeline.pcwBNC1, pipeline.pcwBNCN]:    
    #    print('-----------------')
    #    for w in sorted(s):
    #        print(w, s[w]) 

    #for mw in sorted(pipeline.bmw):
    #    print(mw, '\t'.join(pipeline.bmw[mw]))

    print('-------------------------------------------------------')
    print('Multiple words in Brown to process', len(pipeline.bmw))
    print('Same words, single disagreement', len(pipeline.pcwB1C1))
    print('Same words, Brown adds tag', len(pipeline.pcwB1CN))
    print('Same words, Brown adds many tags', len(pipeline.pcwBNC1))
    print('Same words, multiple disagreements', len(pipeline.pcwBNCN))
    print('Closed Words in only in Brown', len(pipeline.pboClosed))
    print('Open words in only in Brown', len(pipeline.pboOpen))

    
