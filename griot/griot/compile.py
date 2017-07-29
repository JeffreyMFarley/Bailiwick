import sys
import os
import csv
import re
from operator import itemgetter
import griot

PATH_INPUT = r'..\data\b240.txt'
PATH_OUTPUT = r'..\data\COCA.txt'

stagingFileName = r'..\data\staging.txt'

def keep(x):
    ''' removing individual mess-ups
    '''
    if x['w1'] == 'a' and x['c1'] == 'TO':
        return False

    if x['w1'] == 'in' and x['c1'] == 'CS':
        return False

    if x['w1'] == 'on' and x['c1'] == 'JJ':
        return False

    if x['w1'] == 'every' and x['c1'] == 'AT1':
        return False

    if x['L1'] in ['accomodate', 'dom', 'exorcize', 'melded']:
        return False

    if x['w1'] == 'siting' and x['L1'] == 'sit':
        return False

    if x['w1'] == 'strew' and x['L1'] == 'strew' and x['c1'] == 'VVD':
        return False

    if x['w1'] in ['worshiping', 'worshiped']:
        return False

    return True

def secondarySort(x):
    return (re.sub('[-\']', '', x['w1']), 
            len(x['w1']), 
            x['c1'])

def iterateSource(fileName=PATH_INPUT):
    print('Loading', fileName)
    with open(fileName, 'r') as f:
        reader = csv.DictReader(f, dialect=csv.excel_tab)
        for row in reader:
            yield {
                    'w1': row['w1'].lower(),	
                    'L1': row['L1'].lower(), 
                    'c1': row['c1'].upper(),
                    'coca': int(row['coca'])
                    }

def save(data, fileName=stagingFileName):
    print('Saving', fileName)
    with open(fileName, 'w') as f:
        fieldSet = ['w1', 'L1', 'c1', 'coca']
        sep = '\t'

        # write the header row
        header = sep.join([col for col in fieldSet])
        f.writelines([header, '\n'])

        # write the rows
        data.sort(key=secondarySort)                    # secondary sort
        data.sort(key=itemgetter('coca'), reverse=True) # primary sort
        for row in data:
            a = sep.join([str(row[col]) for col in fieldSet])
            f.writelines([a, '\n']) 

#-------------------------------------------------------------------------------
# Main
#-------------------------------------------------------------------------------

if __name__ == '__main__':
    inFile = PATH_INPUT
    outFile = PATH_OUTPUT
    argc = len(sys.argv)
    if argc > 1:
        inFile = sys.argv[1]
    if argc > 2:
        outFile = sys.argv[2]

    # where is this script?
    thisScriptDir = os.path.dirname(__file__)

    # get the absolute paths
    inputFileName = os.path.join(thisScriptDir, inFile)
    cocaFileName = os.path.join(thisScriptDir, outFile)

    lemma = griot.LemmaInflections();
    adds = [griot.Enrich(), lemma]
    updates = [griot.FineTune(), lemma]
    output = []

    # Run the transform operations as a list monad
    for x in filter(keep, griot.iterateSource(inputFileName)):
        val = x
        for s in updates:
            val = s.map(val)
        output.append(val)

    for x in adds:
        output.extend(x)

    # Save the compiled file
    griot.save(output, cocaFileName)


