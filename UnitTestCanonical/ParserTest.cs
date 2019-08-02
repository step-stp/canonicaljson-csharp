using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using Stratumn.Canonical;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using java.math;

namespace UnitTestCanonical
{
    /// <summary>
    ///  @copyright Stratumn
    /// </summary>
    [TestClass]
    public class ParserTest
    {

        [TestMethod]
        public void TestSanity()
        {

            new Parser("{\"a\":\"b\"}").Parse();
            new Parser("{\"string\": \"\\u20ac$\\u000F\\u000aA'\\u0042\\u0022\\u005c\\\\\\\"\\/\"}").Parse();
        }


        List<Datum> valid = new List<Datum>() {

                    new    Datum("false",false,"false")
                          ,new Datum("null",null,"null")
                          ,new Datum("true",true,"true")
                          ,new Datum("100E+100",new BigDecimal("100e100"),"big integers")
                          ,new Datum("-1",new BigDecimal(-1),"negative integers")
                          ,new Datum("1.21e1",new BigDecimal("12.1"),"decimal numbers")
                          ,new Datum("\"\\ufb01\"","ﬁ","unicode encoded characters")
                          ,new Datum("\"\\b\"","\b","short escaped characters")
                          ,new Datum("[]",new List<Object>(),"empty arrays")
                          ,new Datum("[\"a\", 1, true]",new List<Object>(new List<Object>(new Object[] {"a", new BigDecimal(1), true})),"arrays")
                    };


        [TestMethod]
        public void TestValid()
        {
            foreach (Datum dt in valid)
            {
                Object result;

                result = new Parser(dt.Input).Parse();
                Assert.AreEqual(dt.Output == null ? "NULL" : dt.Output.ToString(), result == null ? "NULL" : result.ToString());


            }
        }



        class Datum
        {

            public string Input { get; set; }
            public Object Output { get; set; }
            public string description { get; set; }
            public Datum(String input, Object output, String description)
            {
                this.Input = input;
                this.Output = output;
                this.description = description;
            }


            public override string ToString()
            {
                return "Datum [input=" + Input + ", output=" + Output + ", description=" + description + "]";
            }



        }
    }
}