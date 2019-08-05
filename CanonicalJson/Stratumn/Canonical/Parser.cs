
using java.math;
using Stratumn.Canonical.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stratumn.Canonical
{

    /// <summary>
    /// @ copyright Stratumn
    /// </summary>
    public class Parser
    {

        private static readonly char NULL_CHAR = '\u0000';
        private static readonly Regex BOOLEAN_PATTERN = new Regex("^true|false$", RegexOptions.IgnoreCase);
        private static readonly Regex NUMBER_PATTERN = new Regex("^-?[0-9]+(\\.[0-9]+)?([eE][-+]?[0-9]+)?$", RegexOptions.IgnoreCase);

        private static readonly Regex BAD_NUMBER_PATTERN = new Regex("^-?0\\d+");

        // Regular expressions that matches characters otherwise inexpressible in
        // JSON (U+0022 QUOTATION MARK, U+005C REVERSE SOLIDUS,
        // and ASCII control characters U+0000 through U+001F) or UTF-8 (U+D800 through
        // U+DFFF)
        private static readonly Regex FORBIDDEN = new Regex("[\\u0022\\u005c\\u0000-\\u001F\\ud800-\\udfff]");

        private static readonly Regex HEX_PATTERN = new Regex("^([0-9,a-f,A-F]){4}$", RegexOptions.IgnoreCase);

        /***
         * index and value of the current character
         */
        private int index;
        private char chr;
        /***
         * input json data
         */
        private string jsonData;
        /**
         * object value
         */
        private Object root;

        public Parser(string jsonString)
        {
            this.jsonData = jsonString;
        }

        /***
         * starts parsing returning the object
         * @return
         * @throws IOException
         */
        public Object Parse()
        {
            root = ParseElement();
            Scan();
            if (chr != NULL_CHAR && !isWhiteSpace(chr))
            {
                throw new IOException("Improperly terminated JSON object:" + chr);
            }
            return root;

        }

        /***
         * Initiates parsing next element based on type 
         * @return
         * @throws IOException
         */
        private Object ParseElement()
        {
            switch (Scan())
            { //skipwhite space and find first chr
                case Constants.C_LEFT_BRACKET:
                    return ParseArray();
                case Constants.C_LEFT_CURLY_BRACKET:
                    return ParseObject();
                case Constants.C_DOUBLE_QUOTE:
                    return ParseQuotedString();
                case Constants.NULL_CHAR:
                    throw new IOException("Unexpected end of data reached");

                default:
                    return ParseSimpleType();
            }
        }

        /***
         * Parses an object of the form "{"key":value}" or empty object {}
         * @return
         * @throws IOException
         */
        private Object ParseObject()
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>(new LexComparator());
            bool next = false;
            //chr = { 
            while (Peek() != Constants.C_RIGHT_CURLY_BRACKET)
            {
                if (next)
                {
                    ScanFor(Constants.C_COMMA);
                }
                next = true;
                ScanFor(Constants.C_DOUBLE_QUOTE);
                //chr = "
                string name = ParseQuotedString();
                ScanFor(Constants.C_COLON);
                //chr = : 
                object val = ParseElement();
                if (dict.ContainsKey(name))
                {
                    throw new IOException("Duplicate property: " + name);
                }
                dict.Add(name, val);

            }
            Scan();

            return dict;
        }

        /***
	     * Parses Arrays of the form [X,Y,Z] or empty [] 
	     * @return
	     * @throws IOException
	     */
        private Object ParseArray()
        {

            List<object> array = new List<object>();
            bool next = false;
            //current chr = [ 
            while (Peek() != Constants.C_RIGHT_BRACKET)
            {
                if (next)
                {
                    ScanFor(Constants.C_COMMA);
                }
                else
                {
                    next = true;
                }
                array.Add(ParseElement());
            }
            Scan();
            return array;
        }


        /***
	     * Parses Boolean Numeric and null values
	     * @return
	     * @throws IOException
	     */
        private Object ParseSimpleType()
        {
            StringBuilder tempBuffer = new StringBuilder();
            //construct the token
            tempBuffer.Append(chr);
            char c;
            while ((c = Peek()) != NULL_CHAR &&
                                    c != Constants.C_COMMA &&
                                            c != Constants.C_RIGHT_BRACKET &&
                                                        c != Constants.C_RIGHT_CURLY_BRACKET)
            {
                //whitespace terminates simple types
                if (isWhiteSpace(Next()))
                {
                    break;
                }
                tempBuffer.Append(chr);
            }

            string token = tempBuffer.ToString();
            if (token.Length == 0)
            {
                throw new IOException("Missing argument");
            }
            if ( NUMBER_PATTERN.IsMatch(token))
            {
                if (BAD_NUMBER_PATTERN.IsMatch(token) )
                {
                    throw new IOException("Bad number: leading zero: " + token);
                }
                return new BigDecimal(token);
            }
            else if (BOOLEAN_PATTERN.IsMatch(token))
            {
                return Convert.ToBoolean(token);
            }
            else if (token.Equals("null"))
            {
                return null;
            }
            else
            {
                throw new IOException("Unrecognized or malformed JSON token: " + token);
            }
        }

        /***
         * parse string tokens between two quotes.
         * @return
         * @throws IOException
         */
        private string ParseQuotedString()
        {
            StringBuilder result = new StringBuilder();
            // When parsing for string values, we must look for " and \ characters.
            //current chr = "
            if (chr != Constants.C_DOUBLE_QUOTE)
            {
                throw new IOException("Bad String");
            }
            while (Next() != Constants.C_DOUBLE_QUOTE)
            {
                if (chr < ' ')
                {
                    throw new IOException(chr == '\n' ? "Unterminated string literal" : "Unescaped control character: 0x" + Convert.ToString(chr, 16));
                }
                if (chr == Constants.C_BACK_SLASH)
                {
                    switch (Next())
                    {
                        case Constants.C_DOUBLE_QUOTE:
                        case Constants.C_BACK_SLASH:
                        case Constants.C_FORWARD_SLASH:
                            break;

                        case 'b':
                            chr = Constants.C_BACKSPACE;
                            break;

                        case 'f':
                            chr = Constants.C_FORM_FEED;
                            break;

                        case 'n':
                            chr = Constants.C_LINE_FEED;
                            break;

                        case 'r':
                            chr = Constants.C_CARRIAGE_RETURN;
                            break;

                        case 't':
                            chr = Constants.C_TAB;
                            break;

                        case 'u': // hex
                            chr = ParseHex();
                            break;

                        default:
                            throw new IOException("Unsupported escape:" + chr);
                    }
                }
                result.Append(chr);
            }
            return result.ToString();
        }

        /***
         * Reads 4 characters and attempts to convert hex to char
         * @return
         * @throws IOException 
         */
        private Char ParseHex()
        {
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                hex.Append(Next());
            }
            string hexStr = hex.ToString();
            if (HEX_PATTERN.IsMatch(hexStr) )
            {
                return (char)Convert.ToInt32(hex.ToString(), 16);
            }

            throw new IOException("Bad hex in escape: \\u" + hexStr);
        }


        /***
         * Returns the next non white character without moving the cursor to it.
         * @return
         * @throws IOException
         */
        private Char Peek()
        {
            int bookmark = index;
            char c = Scan();
            index = bookmark;
            chr = jsonData[index - 1];
            return c;
        }


        /***
         * White space check
         * @param c
         * @return
         */
        private Boolean isWhiteSpace(Char c)
        {
            return Char.IsWhiteSpace(c);
        }

        /***
         * Moves to the next nonwhitepace character and tests if that char matches expected
         * @param expected
         * @throws IOException
         */
        private void ScanFor(char expected)
        {
            if (Scan() != expected)
            {
                throw new IOException("Expected '" + expected + "' but got '" + chr + "'");
            }
        }



        /*** 
         * Moves to the next non-whitespace character
         * @return
         * @throws IOException
         */
        private Char Scan()
        {
            while (Next() != NULL_CHAR && isWhiteSpace(chr)) ;
            return chr;
        }

        /***
         * Jumps to the next character 
         * @return next char or null if end is reached
         * @throws IOException
         */
        private Char Next()
        {
            int maxLength = jsonData.Length;
            if (index < maxLength)
            {
                chr = jsonData[index++];
            }
            else
                chr = NULL_CHAR;
            return chr;
        }


    }
}
