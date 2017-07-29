import os
import sys

class Gloss:
    @classmethod
    def isClosedWord(cls, specificTag):
        if specificTag[0] in ['A', 'C', 'D', 'E', 'G', 'I', 
                              'P', 'Q', 'T', 'X', 'Z']:
            return True

        if specificTag in ['JK', 'RG', 'RGA', 'RGQ', 'RGQV', 'RP', 'RPK', 
                           'RRQ', 'RRQV', 'RT', 'VB0', 'VBDR', 'VBDZ', 'VBG',
                           'VBI', 'VBM', 'VBN', 'VBR', 'VBZ', 'VD0', 'VDD', 
                           'VDG', 'VDI', 'VDN', 'VDZ', 'VH0', 'VHD', 'VHG', 
                           'VHI', 'VHN', 'VHZ', 'VM', 'VMK']:
            return True

        return False;

    @classmethod
    def isAgent(cls, specificTag):
        return specificTag[0] in ['E', 'N', 'P']

    @classmethod
    def isModifier(cls, specificTag):
        return specificTag[0] in ['A', 'D', 'J', 'R', 'M']

    @classmethod
    def isGerund(cls, specificTag):
        return specificTag in ['VBG', 'VDG', 'VHG', 'VVG', 'VVGK']

    @classmethod
    def isInfinite(cls, specificTag):
        return specificTag in ['VB0', 'VBG', 'VBN', 'VDG', 'VDN', 'VHG', 'VHN',
                               'VVG', 'VVI', 'VVN', 'VVGK', 'VVNK']

    @classmethod
    def isParticiple(cls, specificTag):
        return specificTag in ['VBG', 'VBN', 'VDG', 'VDN', 'VHG', 'VHN', 
                               'VVG', 'VVN', 'VVGK', 'VVNK']

    @classmethod
    def isPostDeterminer(cls, specificTag):
        return specificTag in ['DA', 'DA1', 'DA2', 'DAR', 'DAT']

    @classmethod
    def isPreDeterminer(cls, specificTag):
        return specificTag in ['DB', 'DB2']