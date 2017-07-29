import sys
import os
import csv
import griot
from operator import itemgetter

PATH_INPUT = r'../data/fine_tune.txt'

class FineTune:
    def __init__(self, inputFileName=PATH_INPUT):
        with open(inputFileName, 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            self.tuning = {(x['w1'], x['c1_orig']): x['c1_new'] for x in reader}

    def map(self, x):
        tuple = (x['w1'], x['c1'])
        if tuple not in self.tuning:
            return x

        x['c1'] = self.tuning[tuple]
        return x
    
    def __str__(self):
        return 'Fine Tune Parts of Speech'

#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

if __name__ == '__main__':
    singleton = FineTune()

    # process
    result = list(map(singleton.map, griot.iterateSource()))
    griot.save(result, griot.stagingFileName)

