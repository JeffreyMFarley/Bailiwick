import os
import sys
import json
import csv
import itertools
from collections import defaultdict
from gloss import Gloss

class QuantifyTags:
    def __init__(self):
        self.byTag = defaultdict(dict)
        self.distribution = defaultdict(dict)
        self.byWord = defaultdict(dict)
        self.inMultiple = set()
        self.tooRareThreshold = 999

    #-------------------------------------------------------------------------------

    def _processPair(self, pair):
        dict1 = self.byTag[pair[0]]
        dict2 = self.byTag[pair[1]]
        shared = set(dict1) & set(dict2)

        for word in shared:
            low = min(dict1[word], dict2[word])
            high = max(dict1[word], dict2[word])
            if (high/low) < self.tooRareThreshold:
                valuePair = (dict1[word], dict2[word])
                self.distribution[pair][word] = valuePair
                self.inMultiple.add(word)

    def _removeShared(self, tag):
        tagDict = self.byTag[tag]
        shared = set(tagDict) & self.inMultiple
        for word in shared:
            self.byWord[word][tag] = tagDict[word]
            del tagDict[word]

    def _addUnambiguous(self, tag):
        nullPair = (tag, tag)
        tagDict = self.byTag[tag]
        for word in tagDict:
            valuePair = (tagDict[word], 0)
            self.distribution[nullPair][word] = valuePair

    #-------------------------------------------------------------------------------

    def _sortBySize(self, k):
        return sum(self.byWord[k].values())

    #-------------------------------------------------------------------------------

    def run(self, brownFileName, cocaFileName, outputDir):
        # extract
        print('Loading Source')
        with open(cocaFileName, 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            for row in reader:
                self.byTag[row['c1'].upper()][row['w1']] = int(row['coca'])

        #with open(brownFileName, 'r') as f:
        #    reader = csv.DictReader(f, dialect=csv.excel_tab)
        #    for row in reader:
        #        self.byTag[row['POS'].upper()][row['Word']] = int(row['Count'])
        
        # record the pairs
        for pair in itertools.combinations(sorted(self.byTag), 2):
            self._processPair(pair)

        # finish processing the tag dictionaries
        for tag in self.byTag:
            self._removeShared(tag)
            self._addUnambiguous(tag)

        # load
        print('Saving Results')
        outputDistribution = os.path.join(outputDir, 'word_classes.txt')
        with open(outputDistribution, 'w') as f:
            for k in sorted(self.distribution):
                kout = k if k[0] != k[1] else k[0]
                if len(self.distribution[k]) < 11:
                    print(kout,list(self.distribution[k]), file=f)
                if len(self.distribution[k]) >= 11:
                    print(kout,len(self.distribution[k]), file=f)

        #vjn = {w for k in sorted(self.distribution) 
        #      if ((k[0] == 'JJ' and k[1] in ['VV0', 'VVD', 'VVN', 'VVG'] ) 
        #          or (k[0] == 'NN1' and k[1] in ['VV0', 'VVD', 'VVN', 'VVG'])
        #          )
        #      for w in self.distribution[k]}

        #outputVJ = os.path.join(outputDir, 'word_vjn.txt')
        #with open(outputVJ, 'w') as f:
        #    for w in sorted(vjn):
        #        print(w, file=f)

        outputWords = os.path.join(outputDir, 'words_in_multiple.txt')
        with open(outputWords, 'w') as f:
            for k in sorted(self.byWord):
                result = [x for x in self.byWord[k] if Gloss.isClosedWord(x)]
                if len(result):
                    print(k, self.byWord[k], file=f)

            print('\n\n', file=f)
            for k in sorted(self.byWord, key=self._sortBySize, reverse=True):
                result = [x for x in self.byWord[k] if Gloss.isClosedWord(x)]
                if len(result):
                    print(k, self._sortBySize(k), file=f, sep='\t')

#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

INPUT_COCA = '../Bailiwick/Corpora/COCA.txt'
INPUT_BROWN = '../Bailiwick/Corpora/BrownFrequency.txt'
OUTPUT = '../data/'

if __name__ == '__main__':
    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    brownFileName = os.path.join(thisScriptDir, INPUT_BROWN)
    cocaFileName = os.path.join(thisScriptDir, INPUT_COCA)
    outputDir = os.path.join(thisScriptDir, OUTPUT)

    # run the jewels
    pipeline = QuantifyTags()
    pipeline.run(brownFileName, cocaFileName, outputDir)

    
