using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Canonical
{
    /// <summary>
    /// @ copyright Stratumn
    /// </summary>
    public class CanonicalJson
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
            return new Transformer(/*gap, indent, replacer*/).Transform(value);
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
    }
}
