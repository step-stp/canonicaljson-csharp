using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.CanonicalJson
{
    /// <summary>
    /// @ copyright Stratumn
    /// </summary>
    public class Canonicalizer
    {
        /**
         * Converts an object to a string representation of type json.
         * Object is Map, Vector, null , String, Boolean,Double.
         * @param value
         * @return
         * @throws IOException
         */
        public static string Stringify(Object value)
        {
            return new Transformer().Transform(value);
        }

        /***
         * Parse string to an Object of type map or vector
         * @param source
         * @return
         * @throws IOException
         */
        public static Object Parse(string source)
        {
            return new Parser(source).Parse();
        }

        /***
        * Parses a string to object and converts it back to string in canonical Json form 
        * @param source
        * @return
        * @throws IOException
        */
        public static String Canonizalize(String source)
        {
            return new Transformer().Transform(new Parser(source).Parse());
        }
    }
}
