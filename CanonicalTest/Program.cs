using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace Stratumn.CanonicalJsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            { 
                Console.WriteLine("Usage: CanonicalJsonTest  <DataFolder | JSON InputFile>");
                Console.WriteLine("       DataFolder/input.json  DataFolder/expected.json");
                Console.WriteLine("If Parameter is a folder, searches for all subfolders with input.json (and expected.json).");
                    Console.WriteLine( "Reads/parses DataFolder/input.json then serializes it to canonical json and write output.json in same folder.");
                    Console.WriteLine( "Compares output to expected.json if found under same folder. ");
                return;
                
            }

            if (File.Exists(args[0]) || Directory.Exists(args[0]))
            {
                new CanonicalJsonTest().ProcessTestFiles(args[0], false);
            }
            Console.ReadLine();
        }
    }
}
