using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;

using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using Cudafy.Types;

namespace Aldyparen
{
    /*
     *      Symbol codes:
     *      
     *      0  instant value
     *      1  +
     *      2  -
     *      3  *
     *      4  /
     *      5  ^
     *      6  lg
     *      7  exp
     *      8  sin     
     *      9  cos
     *      10 tg
     *      11 ctg
     *      12 arcsin
     *      13 arccos
     *      14 arctg
     *      15 arcctg
     *      16 sh   
     *      17 ch
     *      18 th
     *      19 cth
     *      20 abs
     *      21 re
     *      22 im     
     *      23 arg
     *      24 sqrt
     *      25 UNAR MINUS
     *      25-63 RESERVED FOR UNARY OPERATORS
     *      64-127 VARIABLES
     *      128 (
     *      129 )   
     *      130 ERROR
     * 
     * 
     */



    public class Function
    {

        public class Symbol
        {
            public byte code;
            public    Complex value;
            public int posStart; 

            public Symbol(byte _code, Complex _value)
            {
                code = _code;
                value = _value;
            }

            public Symbol(byte _code )
            {
                code = _code;
                value = 0;
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
                return (code>=1 && code<=5);
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
                return (code ==128);
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

        }
         

        public static String[] reserved = new String[]
        {"i","pi","sin","cos","tg","ctg","abs","re","im","arg","arcsin",
            "arccos","arctg","arcctg","exp","lg","sh","ch","th","cth","e","sqrt"};
         
        public List<Symbol> symbols;
        public Dictionary<String, int> varNames = new Dictionary<String, int>();
        public int varCount = 0;
        public Complex[] varValues = new Complex[64];

        private String text = "";

        public byte[] rpnFormula;
        public Complex[] rpnKoef;


        public bool correct = false;
        public String errorMessage;
        public int errorIndex;

        public Function()
        {

        }

        private Symbol evalName(String name, Symbol prevSymbol)
        {
            //firstly, try to evaluate it as a number
            double val;
            try
            {
                val = Convert.ToDouble(name);
                return new Symbol(0, new Complex(val,0));
            }
            catch (Exception ex)
            {
                 //Do nothing, go further
            }


            if (name == "+") return new Symbol(1);
            else if (name == "-")
            {
                if (prevSymbol!=null && (prevSymbol.isNumber() || prevSymbol.isClosingBracket()))       //Binar minus
                    return new Symbol(2);
                else            //Unar Minus
                    return new Symbol(25);
            }
            else if (name == "*") return new Symbol(3);
            else if (name == "/") return new Symbol(4);
            else if (name == "^") return new Symbol(5);

            else if (name == "(") return new Symbol(128);
            else if (name == ")") return new Symbol(129);

            else if (name == "i") return new Symbol(0, Complex.ImaginaryOne);
            else if (name == "e") return new Symbol(0, new Complex(Math.E, 0));
            else if (name == "pi") return new Symbol(0, new Complex(Math.PI, 0));

            else if (name == "lg") return new Symbol(6);
            else if (name == "exp") return new Symbol(7);
            else if (name == "sin") return new Symbol(8);
            else if (name == "cos") return new Symbol(9);
            else if (name == "tg") return new Symbol(10);
            else if (name == "ctg") return new Symbol(11);
            else if (name == "arcsin") return new Symbol(12);
            else if (name == "arccos") return new Symbol(13);
            else if (name == "arctg") return new Symbol(14);
            else if (name == "arcctg") return new Symbol(15);
            else if (name == "sh") return new Symbol(16);
            else if (name == "ch") return new Symbol(17);
            else if (name == "th") return new Symbol(18);
            else if (name == "cth") return new Symbol(19);
            else if (name == "abs") return new Symbol(20);
            else if (name == "re") return new Symbol(21);
            else if (name == "im") return new Symbol(22);
            else if (name == "arg") return new Symbol(23);
            else if (name == "sqrt") return new Symbol(24);


            byte i=64;
            foreach (String s in varNames.Keys)
            {
                if (s == name) return new Symbol(i);
                i++;
            }

            return new Symbol(130);
        }

        public void setText(String f)
        {
            f = f.ToLower().Replace(".", ",")+" ";
            text = f;



            //Parsing string into symbols
            symbols = new List<Symbol>();
            String currentName = "";
            Symbol lastSymbol = null;
            byte mode = 0;//1-name, 2-number

            for (int i = 0; i < f.Length; i++)
            {
                if (f[i] == '+' || f[i] == '-' || f[i] == '*' || f[i] == '/' || f[i] == '^' || f[i] == '(' || f[i] == ')' || f[i]==' ')
                {
                    if (currentName.Length > 0)
                    {
                        lastSymbol = evalName(currentName, lastSymbol);
                        lastSymbol.posStart = i - currentName.Length;

                        if (lastSymbol.code == 130)
                        {
                            correct = false;
                            errorMessage = "Unknown name '" + currentName + "'";
                            errorIndex = lastSymbol.posStart;
                            return;
                        }

                        symbols.Add(lastSymbol);
                        currentName="";
                    }

                    if(f[i]!=' ') 
                    {
                        lastSymbol = evalName(f[i].ToString(), lastSymbol);
                        lastSymbol.posStart = i;
                        symbols.Add(lastSymbol);
                    }
                }
                else if( (f[i]>='a' && f[i]<='z') || (f[i]>='0' && f[i]<='9') || f[i]==',')
                {
                    currentName += f[i];
                }
                else 
                {
                    correct = false;
                    errorMessage = "Unallowable symbol '" + f[i] + "'";
                    errorIndex = i;
                    return;
                }
            }

            
            Console.WriteLine("Symbols:");
            foreach (Symbol s in symbols)
            {
                Console.WriteLine("{0},{1}(2)",s.code, s.value, s.posStart);
            }
             

            //Building postfix notation
            Stack<Symbol> stack = new Stack<Symbol>();
            List<Symbol> output= new List<Symbol>();

            foreach (Symbol s in symbols)
            {
                if (s.isNumber()) output.Add(s);
                else if (s.isUnaryOperator() || s.isOpeningBracket())
                {
                    stack.Push(s);
                }
                else if (s.isClosingBracket())
                {
                    while  (stack.Count != 0 && (!stack.Peek().isOpeningBracket()))
                    {
                         output.Add(stack.Pop());
                    }

                    if (stack.Count == 0)
                    {
                        errorMessage = "Invalid syntax";
                        errorIndex = s.posStart;
                        correct = false;
                        return;
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else if (s.isBinaryOperator())
                {
                    if (s.isRightAssociated())
                    {
                        while (stack.Count != 0 && s.getPriority() < stack.Peek().getPriority())
                        {
                            output.Add(stack.Pop());
                        }
                    }
                    else
                    {
                        while (stack.Count != 0 && s.getPriority() <= stack.Peek().getPriority())
                        {
                            output.Add(stack.Pop());
                        }
                    }

                    stack.Push(s);
                }
            }


            while (stack.Count != 0)
            {
                Symbol s = stack.Pop();
                if (!s.isOperator())
                { 
                    errorMessage="Invalid syntax";
                    errorIndex = s.posStart;
                    correct =  false;
                    return;
                }

                output.Add(s);
            }


            
            Console.WriteLine("RPN:");
            foreach (Symbol s in output)
            {
                Console.WriteLine("{0},{1}(2)", s.code, s.value, s.posStart);
            }
            

            //Checking
            int stackSize = 0;
            foreach (Symbol s in output)
            {
                if (s.isNumber()) stackSize++;
                if (s.isBinaryOperator()) stackSize--;
                if (stackSize == 0)
                {
                    correct = false;
                    errorMessage = "Extra operator";
                    errorIndex = s.posStart;
                    return;
                }
            }

            if (stackSize != 1)
            {
                correct = false;
                errorMessage = "Invalid syntax";
                errorIndex = f.Length-1;
                return;
            }


            //Saving symbols
            int symbolCount = 0;
            int instantCount = 0;

            foreach (Symbol s in output)
            {
                symbolCount++;
                if (s.isInstant()) instantCount++;
            }

            rpnFormula = new Byte[symbolCount];
            rpnKoef = new Complex[instantCount];

            int j = 0;
            for (int i = 0; i < symbolCount; i++)
            {
                rpnFormula[i] = output[i].code;
                if(output[i].isInstant())rpnKoef[j++] = output[i].value;
            }

            correct = true;
        }

        public void changeCoeff(int symbolNumber, float newValue)
        { 
        
        }

        public void addVariable(String name)
        {
            name = name.ToLower();
            if (reserved.Contains(name))
            {
                throw new Exception("Word '" + name + "' is reserved and cannot be used as variable name");
            }

            if (varNames.Keys.Contains(name))
            {
                throw new Exception("Variable '" + name + "' already exists");
            }

            if (varCount >= 64)
            {
                throw new Exception("Cannot add variable. Maximum 64 variables allowed.");            
            }

            varNames[name]=varCount++;
        }

        public void setVariable(String name, Complex value)
        {
            if (!varNames.Keys.Contains(name))
            {
                throw new Exception("Variable '" + name + "' doesn't exist");
            }

            varValues[varNames[name]] = value;
        }



        public Complex eval()
        {
            Complex[] stack = new Complex[rpnFormula.Length];
            int sPtr=-1;
            int vPtr=0;

            for (int i = 0; i < rpnFormula.Length; i++)
            {
                byte v = rpnFormula[i];
                if (v == 0)
                {
                    stack[++sPtr] = rpnKoef[vPtr++];
                }
                else if (v <= 5)
                {
                    if (v == 1) stack[sPtr - 1] = stack[sPtr - 1] + stack[sPtr];
                    else if (v == 2) stack[sPtr - 1] = stack[sPtr - 1] - stack[sPtr];
                    else if (v == 3) stack[sPtr - 1] = stack[sPtr - 1] * stack[sPtr];
                    else if (v == 4) stack[sPtr - 1] = stack[sPtr - 1] / stack[sPtr];
                    else if (v == 5) stack[sPtr - 1] = Complex.Pow(stack[sPtr - 1], stack[sPtr]);

                    sPtr--;
                }
                else if (v <= 10)
                {
                    if (v == 6) stack[sPtr] = Complex.Log(stack[sPtr]);
                    else if (v == 7) stack[sPtr] = Complex.Exp(stack[sPtr]);
                    else if (v == 8) stack[sPtr] = Complex.Sin(stack[sPtr]);
                    else if (v == 9) stack[sPtr] = Complex.Cos(stack[sPtr]);
                    else if (v == 10) stack[sPtr] = Complex.Tan(stack[sPtr]);
                }
                else if (v <= 19)
                {
                    if (v == 11) stack[sPtr] = 1 / Complex.Tan(stack[sPtr]);
                    else if (v == 12) stack[sPtr] = Complex.Asin(stack[sPtr]);
                    else if (v == 13) stack[sPtr] = Complex.Acos(stack[sPtr]);
                    else if (v == 14) stack[sPtr] = Complex.Atan(stack[sPtr]);
                    else if (v == 15) stack[sPtr] = Math.PI - Complex.Atan(stack[sPtr]);
                    else if (v == 16) stack[sPtr] = Complex.Sinh(stack[sPtr]);
                    else if (v == 17) stack[sPtr] = Complex.Cosh(stack[sPtr]);
                    else if (v == 18) stack[sPtr] = Complex.Tanh(stack[sPtr]);
                    else if (v == 19) stack[sPtr] = 1 / Complex.Tanh(stack[sPtr]);
                }
                else if (v <= 25)
                {
                    if (v == 20) stack[sPtr] = stack[sPtr].Magnitude;
                    if (v == 21) stack[sPtr] = stack[sPtr].Real;
                    if (v == 22) stack[sPtr] = stack[sPtr].Imaginary;
                    if (v == 23) stack[sPtr] = stack[sPtr].Phase;
                    if (v == 24) stack[sPtr] = Complex.Sqrt(stack[sPtr]);
                    if (v == 25) stack[sPtr] = -stack[sPtr];
                }
                else if (v <= 63)
                {
                    //RESERVED
                }
                else if (v <= 127)
                {
                    stack[++sPtr] = varValues[v - 64];
                }
                else throw new IndexOutOfRangeException("Invalid symbol code");
            }

            return stack[0];
        }


        [Cudafy]
        private static ComplexF eval(ComplexF[] vars, byte[] formula, float[] koef)
        {
            ComplexF[] stack = new ComplexF[formula.Length];

            return stack[0];
        }

    }
}
