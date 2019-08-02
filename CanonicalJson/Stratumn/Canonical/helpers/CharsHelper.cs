using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Canonical.helpers
{
    /***
    * Compares strings lexicographically
    *  MUST order the members of all objects lexicographically by the UCS (Unicode Character Set) code points of their names
    *  preserving and utilizing the code points in U+D800 through U+DFFF (inclusive) for all lone surrogates
    */
    class LexComparator : IComparer<string>

    {

        public int Compare(string keyA, string keyB)
        {
            int result = 0;
            if (!keyA.Equals(keyB))
                for (int i = 0; i < keyA.Length; i++)
                {
                    if (i > keyB.Length - 1)
                    {
                        result = 1;
                        break;
                    }

                    int aCodePoint = CharHelper.ToCodePoint(keyA, i);
                    int bCodePoint = CharHelper.ToCodePoint(keyB, i);
                    if (aCodePoint != bCodePoint)
                    {
                        result = aCodePoint.CompareTo(bCodePoint);
                        break;
                    }
                }
            // if the first characters are equal and the keyB still have characters
            // means B is greater than A
            if (keyA.Length != keyB.Length && result == 0)
            {
                result = -1;
            }

            return result;
        }


    }
    /// <summary>
    /// Get the Code point  
    /// Try to use the utf32 if found otherwise default int 
    /// </summary>
    class CharHelper
    {

        public static int ToCodePoint(string key, int index)
        {
            int codePoint = 0;
            try
            {
                codePoint = Char.ConvertToUtf32(key, index);
            }
            catch (Exception)
            {
                codePoint = (int)key[index];
            }
            return codePoint;
        }

        public static int ToCodePoint(char high, char low)
        {
            int codePoint = 0;
            codePoint = Char.ConvertToUtf32(high, low);

            return codePoint;
        }
    }
}
