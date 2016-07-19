using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            f.setVariable("x", 1);
            f.setVariable("y", 1);
            f.setVariable("z", 3);


            Console.WriteLine("Result: {0}",f.eval());
        }
    }
}
