using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using Stratumn.CanonicalJson;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTestCanonical
{
    [TestClass]
    public class CanonicalJsonTest
    {
        private static string INPUT = "Resources/input/";
        private static string OUTPUT = "Resources/output/";


        /***
      * Reads both input file and expected file
      * Parse input file, Stringify the output 
      * compares the out of the process to the expected file.

      * @param inputFile
      * @param expectedFile
      * @return
      * @
      */

        private static string[] Test(string inputFile, string expectedFile)
        {

            string rawInput = File.ReadAllText(inputFile, Encoding.UTF8);

            string expected = null;
            if (expectedFile != null)
                expected = File.ReadAllText(expectedFile, Encoding.UTF8).Trim();



            string actual = CanonicalJson.Stringify(CanonicalJson.Parse(rawInput));

            return new String[] { rawInput, expected, actual };
        }

        [TestMethod]
        public void Arrays()
        {
            Console.WriteLine("arrays.json");
            string[] expact = Test(INPUT + "arrays.json", OUTPUT + "arrays.json");
            Assert.AreEqual(expact[1], expact[2]);
        }


        [TestMethod]
        public void French()
        {
            Console.WriteLine("arrays.json");
            String[] expact = Test(INPUT + "french.json", OUTPUT + "french.json");
            Assert.AreEqual(expact[1], expact[2]);
        }


        [TestMethod]
        public void structures()
        {
            Console.WriteLine("structures.json");
            String[] expact = Test(INPUT + "structures.json", OUTPUT + "structures.json");
            Assert.AreEqual(expact[1], expact[2]);
        }

        [TestMethod]
        public void values()
        {
            Console.WriteLine("valuesrrays.json");
            String[] expact = Test(INPUT + "values.json", OUTPUT + "values.json");
            Assert.AreEqual(expact[1], expact[2]);
        }

        [TestMethod]
        public void weird()
        {
            Console.WriteLine("weird.json");
            String[]
            expact = Test(INPUT + "weird.json", OUTPUT + "weird.json");
            Assert.AreEqual(expact[1], expact[2]);
        }

        [TestMethod]
        public void CanonicaljsonSpecTests()
        {
            string rootPath = "resources/test/";

            List<string> testFolders = GetTestFolders(rootPath);

            foreach (string testfolder in testFolders)
            {

                string input = Path.Combine(testfolder, "input.json");
                string expected = Path.Combine(testfolder, "expected.json");

                if (!File.Exists(input))
                {
                    Console.Error.WriteLine("Input   files missing in " + testfolder);
                    continue;
                }
                if (!File.Exists(expected))
                    expected = null;
                try
                {
                    Console.WriteLine(input);

                    String[] expact = Test(input, expected == null ? null : expected);

                    Assert.AreEqual(expact[1], (expact[2]));
                }
                catch (IOException e)
                {
                    throw new Exception("Malformed JSON: " + e.Message);

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
