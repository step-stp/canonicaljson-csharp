using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using Stratumn.Canonical;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTestCanonical
{
    /// <summary>
    /// @copyRight Stratumn
    /// </summary>
    [TestClass]
    public class CanonicalJsonTest
    {
      
        /***
       * Reads both input file and expected file
       * Parse input file , Stringify the output 
       * compares the out of the process to the expected file .
       * @param inputFile
       * @param expectedFile
       * @return
       * @throws IOException
       */
        private string[] ApplyParseStringify(string inputFile, string expectedFile)
        {
            string rawInput = String.Join("", File.ReadAllLines(inputFile, Encoding.UTF8));

            string expected = null;
            if (expectedFile != null)
                expected = String.Join("", File.ReadAllLines(expectedFile, Encoding.UTF8));

            string actual = CanonicalJson.Stringify(CanonicalJson.Parse(rawInput));

            string parent = Path.GetDirectoryName(inputFile);
            File.WriteAllBytes(Path.Combine(parent, "output.json"), Encoding.UTF8.GetBytes(actual));

            return new String[] { rawInput, expected, actual };
        }



        [TestMethod]
        public void CanonicalJsonSpecTests()
        {
            // get folders that contain input / expected json files.
            // ResourcesFolder set in configuration file

            string rootFolder = System.Configuration.ConfigurationManager.AppSettings["ResourcesFolder"];

            ProcessTestFiles(rootFolder, true);
        }

        /***
        * Processes all input files, creates output files in same folder, and compares the output to the expected. 
        * An error is displayed if there is a comparison failure. 
        * An error is displayed if there is a malformed JSON.
        * @param folder
        */
        private void ProcessTestFiles(string folder, bool addLineSeparators)
        {
            List<string> testFolders = null;
            if (File.Exists(folder))
            {
                testFolders = new List<string>();
                testFolders.Add(Path.GetDirectoryName(folder));
            }
            else if (Directory.Exists(folder))
                testFolders = GetTestFolders(folder);

            if (testFolders == null)
                throw new IOException("Please select a valid file / folder");

            foreach (string testfolder in testFolders)
            {
                string expected = Path.Combine(testfolder, "expected.json");

                string input = Path.Combine(testfolder, "input.json");


                if (!File.Exists(input))
                {
                    Console.Error.WriteLine("Input  files missing in " + Path.GetFullPath(testfolder));
                    continue;
                }

                if (!File.Exists(expected))
                    expected = null;

                String[] expact = null;
                try
                {
                    expact = ApplyParseStringify(input, expected == null ? null : expected);
                    if (expact != null && expact[1] != null && !expact[2].Equals(expact[1]))
                    {
                        string message = "Values not equal Expected/Actual @" + testfolder + "\r\n"
                        + (expact[1] != null ? "Expected: \r\n" + (addLineSeparators ? Regex.Replace(expact[1], ",", ",\r\n") : expact[1]) + "\r\n" : "") //expected
                        + "Actual: \r\n"
                        + (addLineSeparators ? Regex.Replace(expact[2], ",", ",\r") : expact[2]);  //actual
                        Console.Error.WriteLine(message);
              
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Malformed JSON: " + e.Message + " @ " + testfolder);
                }
              
            }
        }



        /***
         * recursively finds the folders containing input.json
         * @param parentFolder
         * @return
         */
        private List<string> GetTestFolders(string parentFolder)
        {

            List<string> testFoldersList = new List<string>();
            string[] subFolders = Directory.GetDirectories(parentFolder);
            foreach (string folder in subFolders)
            {

                if (File.Exists(Path.Combine(folder, "input.json")))
                {

                    testFoldersList.Add(folder);

                }
                else
                    testFoldersList.AddRange(GetTestFolders(folder));
            }
            return testFoldersList;
        }

    }
}
