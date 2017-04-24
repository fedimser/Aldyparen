using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Aldyparen
{
    class SymbolicExpression
    {
        public enum Type
        {
            ComplexNumber,
            UnaryFunction,
            BinaryFunction,
            Variable
        }
        public static String DoubleToString(Double x) {
            return x.ToString("F");
        }

        Type type;
        String name;
        Complex value;
        SymbolicExpression operand1;
        SymbolicExpression operand2;
        int priority;

        public SymbolicExpression(Complex value)
        {
            this.type = Type.ComplexNumber;
            this.value = value;
            this.priority = 100;
        }

        public SymbolicExpression(String name)
        {
            this.type = Type.Variable;
            this.name = name;
            this.priority = 100;
        }

        public SymbolicExpression(String name, SymbolicExpression operand)
        {
            this.type = Type.UnaryFunction;
            this.name = name;
            this.operand1 = operand;
            this.priority = 6;
        }


        public SymbolicExpression(String name, SymbolicExpression operand1, SymbolicExpression operand2)
        {
            this.type = Type.BinaryFunction;
            this.name = name;
            this.operand1 = operand1;
            this.operand2 = operand2;
            if (name == "+" || name == "-") priority = 3;
            else if (name == "/" || name == "*") priority = 4;
            else if (name == "^") priority = 5;
        }

        public String toString()
        {
            if (this.type == Type.ComplexNumber)
            {
                if (this.value.Real == 0) return DoubleToString(this.value.Imaginary) + "i";
                else if (this.value.Imaginary == 0) return DoubleToString(this.value.Real);
                else return "(" + DoubleToString(this.value.Real) + "+" + DoubleToString(this.value.Imaginary) + "i)";
            }
            else if (this.type == Type.Variable)
            {
                Console.WriteLine("TS(V): " + this.name);
                return this.name;
            }
            else if (this.type == Type.UnaryFunction){
                return this.name + "(" + this.operand1.toString() + ")";
            }
            else if (this.type == Type.BinaryFunction)
            {
                String v1 = this.operand1.toString();
                String v2 = this.operand2.toString();
                if  (this.operand1.priority < this.priority) v1 = "(" + v1 + ")";
                if  (this.operand2.priority <= this.priority) v2 = "(" + v2 + ")";

                return v1+ this.name + v2;
            }
            else return "";
        }

    }
}
