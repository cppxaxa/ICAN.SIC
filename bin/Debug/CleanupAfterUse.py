import os
import shutil

shutil.rmtree("D:\\Projects\\ICAN\\ICAN.SIC\\bin\\Debug\\WebAssets", True)
shutil.rmtree("D:\\Projects\\ICAN\\ICAN.SIC\\bin\\Debug\\GeoResources", True)
shutil.rmtree("D:\\Projects\\ICAN\\ICAN.SIC\\bin\\Debug\\SIMLHubPlugins", True)
os.remove("CleanupAfterUse.py")