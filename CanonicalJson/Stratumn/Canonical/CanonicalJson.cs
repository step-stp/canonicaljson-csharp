using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.CanonicalJson
{
    /***
     *      
     * Helper class for canonicaliziong JSON
     * TODO support beans and datatypes by integrating GSON library and using generics to identify class type. 
     */
   public class CanonicalJson
    {
        /**
         * Converts an object to a string representation of type json.
         * Object is Map, Vector, null , String, Boolean,Double.
         * @param value
         * @return
         */
        public static string Stringify(Object value)
        {
    	    return new Transformer().Transform(value);
        }

    /***
     * Parse string to an Object of type map or vector
     * @param source
     * @return
     */
    public static Object Parse(string source)
    {
    	return new Parser(source).Parse();
        }
    }
}
