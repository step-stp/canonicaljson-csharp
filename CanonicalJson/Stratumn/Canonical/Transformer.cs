﻿

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using java.math;
using System.Globalization;
using java.text;

namespace Stratumn.CanonicalJson
{


    /***
 * Transformer converts and JSON stream to an object Vector / Map / Java Object
 * 
 *
 */
    public class Transformer
    {

        private StringBuilder buffer;
        /* Regular expressions that matches characters otherwise inexpressible in 
          JSON (U+0022 QUOTATION MARK, U+005C REVERSE SOLIDUS, 
         and ASCII control characters U+0000 through U+001F) or UTF-8 (U+D800 through U+DFFF) */
        private static readonly Regex FORBIDDEN = new Regex("[\\u0022\\u005c\\u0000-\\u001F\\ud800-\\udfff]", RegexOptions.IgnoreCase);

        public string Transform(Object obj)
        {
            buffer = new StringBuilder();
            Serialize(obj);
            return buffer.ToString();
        }

        private void Escape(char c)
        {
            buffer.Append(Constants.C_BACK_SLASH).Append(c);
        }

        /***
          * MUST represent all strings (including object member names) in their minimal-length UTF-8 encoding
           * avoiding escape sequences for characters except those otherwise inexpressible in JSON (U+0022 QUOTATION MARK, U+005C REVERSE SOLIDUS, and ASCII control characters U+0000 through U+001F) or UTF-8 (U+D800 through U+DFFF), and
           * avoiding escape sequences for combining characters, variation selectors, and other code points that affect preceding characters, and
           * using two-character escape sequences where possible for characters that require escaping:
           * \b U+0008 BACKSPACE
           * \t U+0009 CHARACTER TABULATION ("tab")
           * \n U+000A LINE FEED ("newline")
           * \f U+000C FORM FEED
           * \r U+000D CARRIAGE RETURN
           * \" U+0022 QUOTATION MARK
           * \\ U+005C REVERSE SOLIDUS ("backslash"), and
           * using six-character \\u00xx uppercase hexadecimal escape sequences for control characters that require escaping but lack a two-character sequence, and
           * using six-character \\uDxxx uppercase hexadecimal escape sequences for lone surrogates
          * @param value
          */
        private void SerializeString(string value)
        {
            buffer.Append(Constants.C_DOUBLE_QUOTE);
            if (FORBIDDEN.Matches(value).Count == 0)
            {
                buffer.Append(value);
            }
            else
            {
                foreach (char c in value.ToCharArray())
                {
                    if (FORBIDDEN.Matches(Convert.ToString(c)).Count == 0)
                    {
                        buffer.Append(c);
                        continue;
                    }
                    switch (c)
                    {
                        case Constants.C_LINE_FEED:
                            Escape('n');
                            break;
                        case Constants.C_BACKSPACE:
                            Escape('b');
                            break;

                        case Constants.C_FORM_FEED:
                            Escape('f');
                            break;

                        case Constants.C_CARRIAGE_RETURN:
                            Escape('r');
                            break;

                        case Constants.C_TAB:
                            Escape('t');
                            break;

                        case Constants.C_DOUBLE_QUOTE:
                        case Constants.C_BACK_SLASH:
                            Escape(c);
                            break;

                        default:
                            Escape('u');
                            string hex = string.Format("{0:x4}", (int)c);
                            buffer.Append(hex.ToUpper());
                            break;
                    }
                }
            }
            buffer.Append(Constants.C_DOUBLE_QUOTE);
        }

        /***
         *  MUST represent all integer numbers (those with a zero-valued fractional part)
        * without a leading minus sign when the value is zero, and
        * without a decimal point, and
        * without an exponent
        *
        * MUST represent all non-integer numbers in exponential notation
        * including a nonzero single-digit significant integer part, and
        * including a nonempty significant fractional part, and
        * including no trailing zeroes in the significant fractional part (other than as part of a ".0" required to satisfy the preceding point), and
        * including a capital "E", and
        * including no plus sign in the exponent, and
        * including no insignificant leading zeroes in the exponent
         * @param bd
         */

        private void SerializeNumber(string value)
        {

            BigDecimal bd = new BigDecimal(value);
            try
            {  //attempt converting to fixed number
                buffer.Append(bd.toBigIntegerExact().toString());

            }
            catch (java.lang.ArithmeticException e)
            {
                NumberFormat formatter = new DecimalFormat("0.0E0");
                formatter.setMinimumFractionDigits(1);
                formatter.setMaximumFractionDigits(bd.precision());
                string val = formatter.format(bd).Replace("+", "");

                buffer.Append(val);
            }

        }

        /***
 * Attempts to serialize an object of type treemap, collection, null , string , boolean , bigdecimal
 * @param o
 */
        private void Serialize(Object o)
        {
            Debug.WriteLine(o?.ToString());
            if (o is SortedDictionary<string, Object>)
            {

                buffer.Append('{');
                bool next = false;

                foreach (KeyValuePair<string, object> keyValue in ((SortedDictionary<string, object>)o).SetOfKeyValuePairs())
                {
                    if (next)
                    {
                        buffer.Append(',');
                    }
                    next = true;
                    SerializeString(keyValue.Key);
                    buffer.Append(':');
                    Debug.WriteLine(keyValue.Value);

                    Serialize(keyValue.Value);
                }
                buffer.Append('}');
            }
            else
            {


                if (o is List<Object>)
                {
                  
                    buffer.Append('[');
                    bool next = false;

                    foreach (Object value in (List<Object>)o)
                    {
                        if (next)
                        {
                            buffer.Append(',');
                        }
                        next = true;
                        Serialize(value);
                    }
                    buffer.Append(']');
                }
                else
                {
                    if (o == null)
                    {
                        buffer.Append("null");
                    }
                    else
                    {
                        if (o is string)
                        {
                            SerializeString((string)o);
                        }
                        else
                        {
                            if (o is bool?)
                            {
                                buffer.Append(((bool?)o).ToString().ToLower());
                            }
                            else
                            {
                                if (o is double? || o is decimal || o is int? || o is BigDecimal)
                                {
                                    SerializeNumber(o.ToString());
                                }
                                else
                                {
                                    throw new ApplicationException("Unknown object: " + o);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetEncodedString()
        {

            return buffer.ToString();
        }

        public byte[] GetEncodedUTF8()
        {
            return Encoding.UTF8.GetBytes(GetEncodedString());
        }
    }
}