import sys
import os
import csv
import griot
from operator import itemgetter

PATH_INPUT = r'../data/enrich.txt'

class Enrich:
    def __init__(self, inputFileName=PATH_INPUT):
        with open(inputFileName, 'r') as f:
            reader = csv.DictReader(f, dialect=csv.excel_tab)
            self.additions = [{
                               'w1': row['w1'].lower(),	
                               'L1': row['L1'].lower(), 
                               'c1': row['c1'].upper(),
                               'coca': int(row['coca'])
                               } for row in reader]
    
    def __iter__(self):
        self.index = -1
        return self

    def __next__(self):
        self.index += 1
        if self.index >= len(self.additions):
            raise StopIteration
        return self.additions[self.index]

    def __str__(self):
        return 'Add missing words'

#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

if __name__ == '__main__':
    e = Enrich()

    # process
    griot.save(list(e), griot.stagingFileName)

