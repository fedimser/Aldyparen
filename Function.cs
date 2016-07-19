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


        public const int MAX_VARIABLES = 64;
        public const int MAX_STACK_SIZE = 16;

        public static String[] reserved = new String[]
        {"i","pi","sin","cos","tg","ctg","abs","re","im","arg","arcsin",
            "arccos","arctg","arcctg","exp","lg","sh","ch","th","cth","e","sqrt"};
         
        public List<Symbol> symbols;
        public Dictionary<String, int> varNames = new Dictionary<String, int>();
        public Complex[] varValues = new Complex[MAX_VARIABLES];

        private String text = "";

        public byte[] rpnFormula;
        public Complex[] rpnKoef;


        private bool correct = false;
        public String errorMessage;
        public int errorIdx1,errorIdx2;

        public Function()
        {

        }

        public Function(String[] _varNames)
        {
            foreach (String s in _varNames)
            {
                addVariable(s);
            }
        }

        public Function clone()
        {
            Function ret = new Function();
            ret.varNames = new Dictionary<String,int>();
            foreach (KeyValuePair<String, int> entry in this.varNames)
            {
                ret.varNames[entry.Key] = entry.Value;
            }
             
            
            ret.text="NOT ACTUAL. SEE RPN ARRAYS.";
            ret.rpnFormula = new byte[rpnFormula.Length];
            ret.rpnKoef = new Complex[this.rpnKoef.Length];
            

            for (int i = 0; i < rpnFormula.Length; i++)
            {
                ret.rpnFormula[i] = this.rpnFormula[i];
            }
            

            for (int i = 0; i < rpnKoef.Length; i++)
            {
                ret.rpnKoef[i] = this.rpnKoef[i];
            }
             

            return ret;
        }


        public void move(Function new_f, double step)
        {
            int n_old = rpnKoef.Length;
            int n_new = rpnKoef.Length;
            if(n_old!=n_new)return;

            for (int i = 0; i < n_old;i++ )
            {
                rpnKoef[i] = rpnKoef[i] + step * (new_f.rpnKoef[i] - rpnKoef[i]);
            }
        }

        public void setText(String f)
        {
            f = f.ToLower().Replace(".", ",");

            if (f.Length==0 || f[f.Length - 1] != ' ') f = f + " ";
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
                        lastSymbol = new Symbol(currentName, lastSymbol,varNames);
                        lastSymbol.posStart = i - currentName.Length;
                        symbols.Add(lastSymbol);

                        if (lastSymbol.code == 130)
                        {
                            correct = false;
                            errorMessage = "Unknown name '" + currentName + "'";
                            errorIdx1 = lastSymbol.posStart;
                            errorIdx2 = lastSymbol.posEnd();

                            return;
                        }

                        
                        currentName="";
                    }

                    if(f[i]!=' ') 
                    {
                        lastSymbol = new Symbol(f[i].ToString(), lastSymbol,varNames);
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
                    errorIdx1 = errorIdx2 = i;
                    return;
                }
            }

            if (symbols.Count == 0)
            {
                correct = false;
                errorIdx1 = 0;
                errorIdx2 = 0;
                errorMessage = "Formula is empty";
                return;
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
                        errorIdx1 = s.posStart;
                        errorIdx2 = s.posEnd();
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
                    errorIdx1 = s.posStart;
                    errorIdx2 = s.posEnd();
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
                    errorIdx1 = s.posStart;
                    errorIdx2 = s.posEnd();
                    return;
                }

                if (stackSize > MAX_STACK_SIZE)
                {
                    correct = false;
                    errorMessage = "Too deep :(";
                    errorIdx1= s.posStart;
                    errorIdx2 = s.posEnd();
                    return;
                }
            }

            if (stackSize != 1)
            {
                correct = false;
                errorMessage = "Invalid syntax";
                errorIdx1= errorIdx2 = f.Length-2;
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



        public String getText()
        {
            return text;
        }

        public Symbol getSymbol(int pos)
        {
            foreach (Symbol s in symbols)
            {
                if (s.posStart <= pos && s.posStart + s.content.Length - 1 >= pos) return s;
            }
            return null;
        }

        

        public bool isCorrect()
        {
            return correct;
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

            if (varNames.Count >= MAX_VARIABLES)
            {
                throw new Exception("Cannot add variable. Maximum 64 variables allowed.");            
            }


            varNames[name]=varNames.Count;
        }

       



        public Complex eval(Dictionary<String,Complex> vars)
        {
            foreach (KeyValuePair<String, Complex> entry in vars)
            {
                if (!varNames.Keys.Contains(entry.Key))
                {
                    throw new Exception("Variable '" + entry.Key + "' doesn't exist");
                }

                varValues[varNames[entry.Key]] = entry.Value;
            } 
            

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

      

    }
}
