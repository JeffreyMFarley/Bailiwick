import os
import sys
import json

class TestSimpleSyntax:
    def __init__(self):
        self.reset()

    def reset(self):
        self.result = True
        self.hasAgent = False
        self.hasAction = False
        self.hasDeterminer = False

    def process(self, tag):
        generalTag = tag[0] 
        self.result = self.result and generalTag in ['A', 'D', 'E', 'I', 'P', 'N', 'V', 'X']
        self.hasAgent = self.hasAgent or generalTag in ['E', 'P', 'N']
        self.hasAction = self.hasAction or generalTag in ['V']
        self.hasDeterminer = self.hasDeterminer or generalTag in ['A', 'D']

    def isValid(self):
        return self.result and self.hasAction and self.hasAgent and self.hasDeterminer


class FindSimpleSentences:
    def __init__(self):
        pass

    #--------------------------------------------------------------------------

    def isSimpleSentence(self, listOfTaggedWords):
        test = TestSimpleSyntax()
        for w0 in listOfTaggedWords:
            if 'w' in w0:
                test.process(w0['t'])
            else:
                for t in w0['ts']:
                    test.process(t)

        return test.isValid()

    def hasMissingVVG(self, listOfTaggedWords):
        for w0 in listOfTaggedWords:
            if 'w' in w0 and w0['t'] == 'VVG' and w0['w'].lower() in ['taking',
                                                                      'saying']:
                return True

        return False

    #--------------------------------------------------------------------------

    def run(self, inputFileName, outputFileName):
        # extract
        print('Loading Source')
        fullCorpus = {}
        with open(inputFileName, 'r') as f:
            fullCorpus = json.load(f)
        
        # filter
        self.subset = {k:v for k,v in fullCorpus.items() 
                       if self.hasMissingVVG(v) }

        # load
        print('Saving Results')

#------------------------------------------------------------------------------
# Main
#------------------------------------------------------------------------------

INPUT = '../Tests/Training/BrownCorpusClaws.json'
OUTPUT = '../Tests/Training/SimpleSentences.json'

if __name__ == '__main__':
    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    inputFileName = os.path.join(thisScriptDir, INPUT)
    outputFileName = os.path.join(thisScriptDir, OUTPUT)

    # run the jewels
    pipeline = FindSimpleSentences()
    pipeline.run(inputFileName, outputFileName)

    for k in sorted(pipeline.subset):
        print('"'+k+'",')


    