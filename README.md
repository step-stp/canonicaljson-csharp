# canonicaljson-csharp
C Sharp library for producing JSON in canonical format. 

1- Download and build the application
2- Run the unit tests available in UnitTestCanonical project
3- You can also run command prompt for executable under UnitTestCanonical project as follows:

UnitTestCanonical.exe "path for folder"

All subfolders with an input.json (and expected.json) will be picked up and processed as follows: \
  1- The file input.json will be parsed 
  2- The resulting object will be stringify-ed 
  3- If expected.json is available it will be compared to the result string. 
  4- An output.json will be created with the result of the process.

or

RunTest.cmd [put path for JSON file]

The input file will be parsed and the result object stringify-ed and an output.json will be created in the same folder.
