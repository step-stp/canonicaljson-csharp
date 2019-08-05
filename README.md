# canonicaljson-csharp
C Sharp library for producing JSON in canonical format. 

1- Build the application using visual studio and run the unit tests. Make sure to set the "ResourcesFolder" key in the application config file.

  Or
  
2- Compile the code and run the command prompt for test project executable as follows:

UnitTestCanonical.exe "full path for folder"

All subfolders with an input.json (and expected.json) will be picked up and processed as follows: \
  1- The file input.json will be parsed 
  2- The resulting object will be stringify-ed 
  3- If expected.json is available it will be compared to the result string. 
  4- An output.json will be created with the result of the process.

or

RunTest.cmd [put path for JSON file]

The input file will be parsed and the result object stringify-ed and an output.json will be created in the same folder.
