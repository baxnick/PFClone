from zipfile import ZipFile
import glob
import os

# Source: http://stackoverflow.com/questions/458436/adding-folders-to-a-zip-file-using-python
def addFolderToZip(myZipFile,folder,prefixPath):
    for file in glob.glob(folder+"/*"):
            if os.path.isfile(file):
                myZipFile.write(file, prefixPath + "/" + os.path.relpath(file))
            elif os.path.isdir(file):
                addFolderToZip(myZipFile,file,prefixPath)


with ZipFile('pushfight.zip', 'w') as myzip:
    myzip.write('pushfight.exe', 'pushfight/pushfight.exe')
    myzip.write('board.txt', 'pushfight/board.txt')
    myzip.write('ai.mgbmp', 'pushfight/ai.mgbmp')
    myzip.write('ai.mgbmr', 'pushfight/ai.mgbmr')
    myzip.write('ai.mgdat', 'pushfight/ai.mgdat')
    myzip.write('ai.mgidx', 'pushfight/ai.mgidx')
    myzip.write('ai.mgrec', 'pushfight/ai.mgrec')
    addFolderToZip(myzip, "pushfight_Data","pushfight")
