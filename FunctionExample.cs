using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;

namespace Aldyparen
{
    class FunctionExample
    {
        public static void example()
        {

            Function f = new Function();
            f.addVariable("x");
            f.addVariable("y");
            f.addVariable("z");

            f.setText("(x+y)*z+cos(0)/0.5^2");

            Dictionary<String, Complex> vars = new Dictionary<string, Complex>();
            vars["x"] = 1;
            vars["x"] = 1;
            vars["x"] = 3;



            Console.WriteLine("Result: {0}",f.eval(vars));
        }
    }
}
