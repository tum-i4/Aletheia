## Aletheia
### 1. Requirement
a) Matlab Runtime 2016a<br />
b) OpenCppCoverage, [has to be added in the path]<br />
c) OpenCV project<br />


### 2. Getting Aletheia
Run command ```git clone https://github.com/tum-i22/Aletheia.git```<br />
Then ```cd Aletheia```<br />
You can find the visual studio solution file threre, open it and build the solution. <br />
You can get the binaries in bin folder. dll files are necessary. The tool is portable, if you copy the exe file along with dll files it will work. 

### 3. Components
Alethia has three modules
* Data Generation
* Failure Clustering
* Fault Localization

Help can be obtained by executing ```Aletheia.exe do=getHelp```
A sample command to generate Hit Spectra matrix is ```Aletheia.exe do=GenerateHitSpectra separator=; project_path=path\to\*.vcxproj file source_directory=path\to\source degreeofparallelism=12 gtest_path=path\to\gtest.exe ```

**For Clustering and Fault Localization, it is necesaary that the first column be ```Index``` and the last column be ```Result```**
Sample command to do clustering on a hit spectra matrix is ```Aletheia.exe do=cluster separator=, input_path=path\to\your\data.csv  clustering_method=maxclust linkage_method=average linkage_metric=euclidean ```

Sample command to do fault localization is ```Aletheia.exe do=faultLocalization separator=; input_path=path\to\your\data.csv ranking_metric=Tarantula``` Different ranking metrics can be used for fault localization, Our tool allows the following ranking metrics. 
* Dstar
* Dstar2
* Dstar3
* Dstar4
* Jaccard
* Tarantula
* Ochiai

If you do not provide any ranking metric, Jaccard will be used as default Ranking metric. 

