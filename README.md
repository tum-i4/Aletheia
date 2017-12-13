## Aletheia
A framework for failure diagnosis

### 1. Requirements
a) Matlab Runtime 2016a <br /> 
b) OpenCppCoverage tool, [has to be added in the System path variable]<br />



### 2. Getting Aletheia
Run command ```git clone https://github.com/tum-i22/Aletheia.git```<br />
Then ```cd Aletheia```<br />
Find the visual studio solution file, open it and build the solution. <br />
Or copy the exe file along with dll files. Aletheia is portable. Find the binaries in bin folder. dll files are needed. 

### 3. Components
Alethia has three modules
* Data Generation: to generte hit/count spectra
* Failure Clustering: to cluster failing tests with respect to hypothesized faults
* Fault Localization: to localize the faults

Help can be obtained by executing:

```Aletheia.exe do=getHelp```

A sample command to generate Hit Spectra:

```Aletheia.exe do=GenerateHitSpectra separator=; project_path=path\to\*.vcxproj file source_directory=path\to\source degreeofparallelism=12 gtest_path=path\to\gtest.exe ```

**For Clustering and Fault Localization, the first column of spectrum csv file must be labeled as ```Index``` and the last column must be labeled as ```Result```**

Sample command to do clustering on a hit spectrum:

```Aletheia.exe do=cluster separator=, input_path=path\to\your\data.csv  clustering_method=maxclust linkage_method=average linkage_metric=euclidean ```

Sample command to do fault localization:

```Aletheia.exe do=faultLocalization separator=; input_path=path\to\your\data.csv ranking_metric=Tarantula``` 

Different ranking metrics can be used for fault localization, Our tool allows the following ranking metrics: 
Dstar1, Dstar2, Dstar3, Dstar4, Jaccard, Tarantula and Ochiai.
If you do not choose any ranking metric, Jaccard will be used as the default. 

### 4. Plugin / VS Extension
There is a seperate Visual Studio Extension project located in https://github.com/tum-i22/AletheiaPlugin . 



### 5. Contact
In case of question, simply email us at

Mojdeh Golagha (golagha@in.tum.de) <br />
Abu Mohammed Raisuddin (am.raisuddin@tum.de)
