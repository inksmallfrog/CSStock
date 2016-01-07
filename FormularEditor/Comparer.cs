using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEditor
{
    public enum Symbol
    {
        numbers,//0
        attributes,//1
        operators,//2
        bracket,//3
        comma,//4
        end,//5
        func,//6
        wrong//7
    }
    class Comparer{
        static private String[] attr = { "high", "low", "vol", "open", "close" };
        static private String[] sym = { "+", "-", "*", "/", "(", ")"};
        static private String[] func = { "if", "pow" ,"acos","atan","asin","cos","sin","tan","exp","ln","log","sqrt","abs","ceiling","floor","intpart","fracpart","round","sign","mod","rand","max","min","nif"};
        static private String[] brac = { "[", "]" };
        static private String[] com = { "," };
        static private String[] endSym = { ";" };

        public static Symbol typeOfElement(String element){
            Boolean isAttr = false;
            //Boolean isNum = false;
            Boolean isBrac = false;
            Boolean isCom = false;
            Boolean isEnd = false;
            Boolean isOperator = false;
            Boolean isFunc=false;
            try {
                Convert.ToDecimal(element);
                //test
                //Console.WriteLine("{0} is num\n", element);
                return Symbol.numbers;
            }
            catch (Exception e) {
            }
            finally {
                foreach (String s in attr) {
                    if (element.Equals(s)) {
                        isAttr = true;
                        //test
                        //Console.WriteLine("{0} is attr\n", element);
                        break;
                    }
                }
                foreach (String s in sym) {
                    if (element.Equals(s)) {
                        isOperator = true;
                        //test
                        //Console.WriteLine("{0} is sym\n", element);
                        break;
                    }
                }
                foreach (String s in brac) {
                    if (element.Equals(s)) {
                        isBrac = true;
                        //test
                        //Console.WriteLine("{0} is brac\n", element);
                        break;
                    }
                }
                foreach (String s in com) {
                    if (element.Equals(s)) {
                        isCom = true;
                        //test
                        //Console.WriteLine("{0} is com\n", element);
                        break;
                    }
                }
                foreach (String s in endSym) {
                    if (element.Equals(s)) {
                        isEnd = true;
                        //test
                        //Console.WriteLine("{0} is end\n", element);
                        break;
                    }
                }
                foreach (String s in func) {
                    if (element.Equals(s)) {
                        isFunc = true;
                        //test
                        //Console.WriteLine("{0} is func\n", element);
                        break;
                    }
                }
            }
            if (isAttr) return Symbol.attributes;
            if (isBrac) return Symbol.bracket;
            if (isCom) return Symbol.comma;
            if (isEnd) return Symbol.end;
            if (isOperator) return Symbol.operators;
            if(isFunc) return Symbol.func;
            else return Symbol.wrong;
        }
    }
}
