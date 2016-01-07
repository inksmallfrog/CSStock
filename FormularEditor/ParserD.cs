using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using StockInfo;

namespace FormulaEditor
{
    class ParserD
    {
        private String[] formulaElement;
        private IStock stock;
        public ParserD(String formula, IStock _stock)
        {
            stock = _stock;
            Regex split = new Regex("([+\\-*/<>=()])|([\\[\\],])|[;]|(AND)|(OR)");
            formulaElement=split.Split(formula);
            //清除空字符串
            int count = 0;
            //for (int i = 0; i < formulaElement.Length; i++) {
            //    if (formulaElement[i] == "") {
            //        for (int j = i + 1; j < formulaElement.Length; j++) {
            //            formulaElement[j - 1] = formulaElement[j];
            //        }
            //        count++;
            //        formulaElement[formulaElement.Length - count] = null;
            //    }
            //}
            foreach (String s in formulaElement) {
                if (s == "") {
                    count++;
                }
            }
            String[] temp = new String[formulaElement.Length - count];
            int countOfTemp=0;
            for (int i = 0; i < formulaElement.Length; i++) {
                if (formulaElement[i] != "") {
                    temp[countOfTemp]=formulaElement[i];
                    countOfTemp++;
                }
            }
                //for (int i = 0; i < temp.Length; i++) {
                //    temp[i] = formulaElement[i];
                //}
            formulaElement = temp;
            //test
            //Decimal r=getResult(formulaElement);
            //Console.WriteLine(r);

            //test
            //String[] test = { "[", "pow", "[", "30", ",", "open", "]", "+", "4", ",", "end", "]" };
            //Queue<String>q=new Queue<string>();
            //foreach (String s in test) {
            //    q.Enqueue(s);
            //}
            //breakFormula(q,",");

            //this.getResult(formulaElement);
            //foreach (String s in formulaElement) {
            //    Console.WriteLine(s);
            //    Comparer.typeOfElement(s);
            //}
            //simplifyForm(formulaElement);
        }
        public Decimal getResult()
        {
            Decimal result=getResult(formulaElement);
            return result;
        }
        
        //计算
        private Decimal getResult(String[] formula) {
            Decimal result = 0;
            formula = simplifyForm(formula);
            Queue<String> midExpression = changeToMid(formula);
            Stack<String> temp = new Stack<String>();

            ////test
            //String[] t = { "4", "2", "*", "8", "8", "-", "2", "3", "+" ,"/","+"};
            //foreach (String s in t) midExpression.Enqueue(s);
            ////end test

            while (midExpression.Count() != 0) {
                String s = midExpression.Dequeue();
                if (Comparer.typeOfElement(s) == Symbol.numbers) {
                    temp.Push(s);
                }
                else if (s.Equals("+")) {
                    Decimal o1;
                    try{
                        o1 = Convert.ToDecimal(temp.Pop());
                    }
                    catch
                    {
                        throw new Exception("表达式错误！请检查基本运算符前是否缺失数据！");
                    }
 
                    Decimal o2;
                    try
                    {
                        o2 = Convert.ToDecimal(temp.Pop());
                    }
                    catch
                    {
                        throw new Exception("表达式错误！请检查基本运算符后是否缺失数据！");
                    }
                    Decimal res = o2 + o1;
                    temp.Push(res.ToString());
                }
                else if (s.Equals("-")) {
                    Decimal o1 = Convert.ToDecimal(temp.Pop());
                    Decimal o2 = Convert.ToDecimal(temp.Pop());
                    Decimal res = o2 - o1;
                    temp.Push(res.ToString());
                }
                else if (s.Equals("*")) {
                    Decimal o1 = Convert.ToDecimal(temp.Pop());
                    Decimal o2 = Convert.ToDecimal(temp.Pop());
                    Decimal res = o2 * o1;
                    temp.Push(res.ToString());
                }
                else if (s.Equals("/")) {
                    Decimal o1 = Convert.ToDecimal(temp.Pop());
                    Decimal o2 = Convert.ToDecimal(temp.Pop());
                    Decimal res = o2 / o1;
                    temp.Push(res.ToString());
                }
            }
            //test
            //Console.WriteLine(temp.Pop());
            try
            {
                result = Convert.ToDecimal(temp.Pop());
            }
            catch
            {
                throw new Exception("表达式错误！请检查是否写入了无法识别的符号！");
            }
            return result;
        }
        //转换后序表达式
        private Queue<String> changeToMid(String[] formulaElement){
            Queue<String> midExpression=new Queue<String>();
            Stack<String> oparetor = new Stack<String>();

            ////test
            //String[]t={"2","*","(","(","3","+","4",")","/","(","5","-","6",")","-","7",")"};
            //formulaElement=t;
            ////endtest

            foreach (String s in formulaElement){
                if(Comparer.typeOfElement(s)==Symbol.numbers||Comparer.typeOfElement(s)==Symbol.attributes){
                    midExpression.Enqueue(s);
                }
                else if(oparetor.Count() == 0 ||icp(s)>isp(oparetor.Peek())){
                    oparetor.Push(s);
                }
                else if(s.Equals(")")){
                    while(!oparetor.Peek().Equals("(")){
                        midExpression.Enqueue(oparetor.Pop());
                    }
                    oparetor.Pop();
                }
                else if (icp(s) < isp(oparetor.Peek())) {
                    while (oparetor.Count()!=0&& icp(s) < isp(oparetor.Peek())) {
                        midExpression.Enqueue(oparetor.Pop());
                    }
                    oparetor.Push(s);
                }
               
            }
            while(oparetor.Count()!=0){
                midExpression.Enqueue(oparetor.Pop());
            }
            ////test
            //while(midExpression.Count()!=0){
            //    Console.Write(midExpression.Dequeue());
            //    Console.WriteLine();
            //}
            return midExpression;
        } 
        //辅助函数 栈内优先级inStackPriority，栈外优先级incomingPriority
        private int isp(String s) {
            if (s.Equals("+") || s.Equals("-")) return 3;
            if (s.Equals("*") || s.Equals("/")) return 5;
            if (s.Equals(")")) return 7;
            if (s.Equals("(")) return 1;
            return -1;
        }
        private int icp(String s) {
            if (s.Equals("+") || s.Equals("-")) return 2;
            if (s.Equals("*") || s.Equals("/")) return 4;
            if (s.Equals(")")) return 1;
            if (s.Equals("(")) return 7;
            return -1;
        }
        //寻找并计算函数值以简化表达式
        private String[] simplifyForm(String[] formula) {
            Queue<String> qu = new Queue<String>();
            foreach (String s in formula) {
                qu.Enqueue(s);
            }
            return simplifyForm(qu);
        }
        private String[] simplifyForm(Queue<String>formula) {
            Queue<String> result = new Queue<String>();
            while (formula.Count()!=0) {
                String temp = formula.Dequeue();
                //如果是函数
                if (Comparer.typeOfElement(temp) == Symbol.func) {
                    Queue<String> qu = new Queue<String>();
                    qu.Enqueue(temp);
                    //获取函数【】内内容（关键在于多个括号的计数）
                    int count=0;
                    do {
                        try
                        {
                            if (formula.Peek().Equals("["))
                            {
                                count++;
                            }
                            else if (formula.Peek().Equals("]"))
                            {
                                count--;
                            }
                        }
                        catch
                        {
                            throw new Exception("表达式错误！请检查函数后是否用[,]标记参数");
                        }
                        qu.Enqueue(formula.Dequeue());
                    } while (count != 0);
                    temp = calculateForm(qu);
                }
                else if (Comparer.typeOfElement(temp) == Symbol.attributes) {
                    if (temp.Equals("vol")) {
                        //do something
                        temp = stock.CurrentDayNode.volumn.ToString();
                    }
                    else if (temp.Equals("open")) {
                        temp = stock.CurrentDayNode.open.ToString();
                    }
                    else if (temp.Equals("close")) {
                        //do something
                        temp = stock.CurrentDayNode.close.ToString();
                    }
                    else if (temp.Equals("high")) {
                        //do something
                        //test
                        temp = stock.CurrentDayNode.high.ToString();
                    }
                    else if (temp.Equals("low")) {
                        //do something
                        //test
                        temp = stock.CurrentDayNode.low.ToString();
                    }
                }
                //若是函数或属性，则现在temp已经成为函数的结果（要加上哪一股票这个参数）；若不是，则temp无需简化
                result.Enqueue(temp);

            }
            String[] simp = new String[result.Count()];
            int c = result.Count();
            for (int i = 0; i < c; i++) {
                simp[i] = result.Dequeue();
            }
            return simp;
        }
        private String calculateForm(Queue<String> formula) {
            ////test
            //String s = formula.Dequeue();
            //while (formula.Count() != 0) {
            //    s += formula.Dequeue();
            //}
            //Console.WriteLine(s);

            //匹配各种函数： formula的形式是：pow[o1,o2]
            if (formula.Peek().Equals("pow")) {
                formula.Dequeue();
                String[] t = breakFormula(formula,",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                String result=Function.pow(o1, o2).ToString();
                return result;
            }
            else if (formula.Peek().Equals("if")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, ",");
                FormulaGetter f = new FormulaGetter(t.ToString(), stock);
                string o0 = f.getResult();
                t = breakFormula(formula, ",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                if (o0.Equals("true")) {
                    return o1.ToString();
                }
                else {
                    return o2.ToString();
                }
            }
            else if (formula.Peek().Equals("nif")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, ",");
                FormulaGetter f = new FormulaGetter(t.ToString(), stock);
                string o0 = f.getResult();
                t = breakFormula(formula, ",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                if (o0.Equals("false")) {
                    return o1.ToString();
                }
                else {
                    return o2.ToString();
                }
            }
            else if (formula.Peek().Equals("acos")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.ACOS(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("asin")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.ASIN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("atan")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.ATAN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("sin")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.SIN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("cos")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.COS(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("tan")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.TAN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("exp")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.EXP(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("ln")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.LN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("log")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.LOG(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("sqrt")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.SQRT(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("abs")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.ABS(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("ceiling")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.CEILING(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("floor")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.FLOOR(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("intpart")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.INTPART(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("rand")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.RAND(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("sign")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.SIGN(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("round")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.ROUND(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("fracpart")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, "]");
                Decimal o = getResult(t);
                String result = Function.FRACPART(o).ToString();
                return result;
            }
            else if (formula.Peek().Equals("max")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, ",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                String result = Function.MAX(o1, o2).ToString();
                return result;
            }
            else if (formula.Peek().Equals("min")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, ",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                String result = Function.MIN(o1, o2).ToString();
                return result;
            }
            else if (formula.Peek().Equals("mod")) {
                formula.Dequeue();
                String[] t = breakFormula(formula, ",");
                Decimal o1 = getResult(t);
                t = breakFormula(formula, "]");
                Decimal o2 = getResult(t);
                String result = Function.MOD(o1, o2).ToString();
                return result;
            }
            return new String ('1',1);
        }
        //工具函数，用以拆分出o1,o2，其中传入的表达式是【o1，o2】或，o2】（或，o2，o3】），end为停止符，为，或】
        private String[] breakFormula(Queue<String> formula,String end) {
            //[或，出栈
            formula.Dequeue();
            //获取o1的值,注意o1可能是一个表达式
            Queue<String> temp = new Queue<String>();
            int count = 0;//[]计数器
            while (true) {
                try
                {
                    if (formula.Peek().Equals("["))
                    {
                        count++;
                    }
                    else if (formula.Peek().Equals("]"))
                    {
                        count--;
                        if (count > -1 && end.Equals("]"))
                        {
                            temp.Enqueue(formula.Dequeue());
                            continue;
                        }
                    }
                    if (!formula.Peek().Equals(end))
                    {
                        temp.Enqueue(formula.Dequeue());
                    }
                    else if (count > 0)
                    {
                        temp.Enqueue(formula.Dequeue());
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    throw new Exception("表达式错误！请检查函数后是否用[,]标记参数");
                }
                
            }
            String[] t = new String[temp.Count()];
            for (int i = 0; i < t.Length; i++) t[i] = temp.Dequeue();

            ////test
            //String s="";
            //foreach (String i in t) { s += i; }
            //Console.WriteLine(s);
           
            return t;
        }
    }
}
