using java.math;
using java.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canonical
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal dec = 3.14M;
            BigDecimal bd = new BigDecimal("3.14");
            NumberFormat formatter = new DecimalFormat("0.0E0");
            formatter.setMinimumFractionDigits(1);
            formatter.setMaximumFractionDigits(bd.precision());
            String val = formatter.format(bd).Replace("+", "");

            Console.WriteLine(bd.precision());

            Console.WriteLine(string.Format("{0:0.0E+0}", dec));
            Console.WriteLine(val);

            Console.ReadLine();
            
        }
    }
}
