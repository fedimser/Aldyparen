using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;

namespace Aldyparen
{
    public class Symbol
    {
        public readonly String content;
        public byte code;
        public Complex value;
        public int posStart;

        /*
        public Symbol(byte _code, Complex _value)
        {
            code = _code;
            value = _value;
        }

        public Symbol(byte _code)
        {
            code = _code;
            value = 0;
        }

        */

        public Symbol(String name, Symbol prevSymbol, Dictionary<String,int> varNames)
        {
            content = name; 

            //firstly, try to evaluate it as a number
            double val;
            try
            {
                val = Convert.ToDouble(name);
                code=0;
                value = new Complex(val,0);
                return;
            }
            catch (Exception ex)
            {
                 //Do nothing, go further
            }

            code=130;


            if (name == "+") code = 1;
            else if (name == "-")
            {
                if (prevSymbol != null && (prevSymbol.isNumber() || prevSymbol.isClosingBracket()))       //Binar minus
                    code = 2;
                else            //Unar Minus
                    code = 25;
            }
            else if (name == "*") code = 3;
            else if (name == "/") code = 4;
            else if (name == "^") code = 5;

            else if (name == "(") code = 128;
            else if (name == ")") code = 129;

            else if (name == "i") { code = 0; value = Complex.ImaginaryOne; }
            else if (name == "e"){ code = 0; value =   new Complex(Math.E, 0);}
            else if (name == "pi") { code = 0; value = new Complex(Math.PI, 0); }

            else if (name == "lg") code = 6;
            else if (name == "exp") code = 7;
            else if (name == "sin") code = 8;
            else if (name == "cos") code = 9;
            else if (name == "tg") code = 10;
            else if (name == "ctg") code = 11;
            else if (name == "arcsin") code = 12;
            else if (name == "arccos") code = 13;
            else if (name == "arctg") code = 14;
            else if (name == "arcctg") code = 15;
            else if (name == "sh") code = 16;
            else if (name == "ch") code = 17;
            else if (name == "th") code = 18;
            else if (name == "cth") code = 19;
            else if (name == "abs") code = 20;
            else if (name == "re") code = 21;
            else if (name == "im") code = 22;
            else if (name == "arg") code = 23;
            else if (name == "sqrt") code = 24;


            byte i=64;
            foreach (String s in varNames.Keys)
            {
                if (s == name) {code=i;return;}
                i++;
            } 
        }

      


        public bool isInstant()
        {
            return (code == 0);
        }

        public bool isNumber()
        {
            return (code == 0 || (code >= 64 && code <= 127));
        }



        public bool isBinaryOperator()
        {
            return (code >= 1 && code <= 5);
        }

        public bool isUnaryOperator()
        {
            return (code >= 6 && code <= 63);
        }

        public bool isOperator()
        {
            return (code >= 1 && code <= 63);
        }


        public bool isOpeningBracket()
        {
            return (code == 128);
        }

        public bool isClosingBracket()
        {
            return (code == 129);
        }

        public int getPriority()
        {
            if (isUnaryOperator()) return 6;
            if (code == 5) return 5;
            else if (code == 4 || code == 3) return 4;
            else if (code == 2 || code == 1) return 3;
            return 0;
        }

        public bool isRightAssociated()
        {
            return false;
        }


        public int posEnd()
        {
            return posStart + content.Length - 1;
        }
    }
}
