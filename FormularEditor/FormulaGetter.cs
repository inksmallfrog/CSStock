using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using StockInfo;
using System.Data.SqlClient;

namespace FormulaEditor
{
    class FormulaGetter {
        private String []m_formula;
        private String m_result;
        private IStock stock;
        public FormulaGetter(String formula, IStock _stock){
            stock = _stock;
            formula = formula.Replace(" ", "");
            Regex split = new Regex("([{}])|[;]|(AND)|(OR)");
            m_formula=split.Split(formula);

            string sql;
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection("Integrated Security=SSPI;Initial Catalog=stock;Data Source=localhost;");
            sql = "select * from " + "formulaTB" + ";";
            cmd.Connection = con;
            cmd.CommandText = sql;
            SqlDataReader reader = cmd.ExecuteReader();
            List <Formula>f=new List<Formula>();
            int account = 0;
            while (reader.Read()) {
                Formula t= new Formula();
                t.title = (string)reader["formulaTitle"];
                t.formula = (string)reader["formula"];
                f.Add(t);
            }
            reader.Close();
            bool exband = false;
            account = 0;
            foreach(Formula t in f){
                for (int i = 0; i < m_formula.Length;i++ ) {
                    if (m_formula[i].Equals(t.title)) {
                        m_formula[i] = t.formula;
                        exband = true;
                    }
                }
            }
            if (exband) {
                string newf = m_formula.ToString();
                m_formula = new FormulaGetter(newf, _stock).getFormula();
            }


             int count = 0;
            foreach (String s in m_formula) {
                if (s == "") {
                    count++;
                }
            }
            String[] temp = new String[m_formula.Length - count];
            int countOfTemp=0;
            for (int i = 0; i < m_formula.Length; i++) {
                if (m_formula[i] != "") {
                    temp[countOfTemp]=m_formula[i];
                    countOfTemp++;
                }
            }
            m_formula = temp;
            //test
            //foreach(string i in m_formula)
            //Console.WriteLine(i);
            m_result= simplyFormula(m_formula);
        }
        public string[]getFormula(){
            return m_formula;
        }
        public String getResult() {
            return m_result;
        }
        private String simplyFormula(String []formula) {
            Queue<String> qFormula = new Queue<String>();
            Queue<String> temp = new Queue<String>();
            Queue<String> result = new Queue<String>();
            foreach (string s in formula) {
                qFormula.Enqueue(s);
            }
            while(qFormula.Count!=0){
                 if(qFormula.Peek().Equals("{")){
                    qFormula.Dequeue();
                    temp.Clear();
                    try { 
                        while (!qFormula.Peek().Equals("}")) {
                            temp.Enqueue(qFormula.Dequeue());
                        }
                    }
                    catch (Exception e)
                    {
                        return "表达式错误！请检查括号是否匹配！";
                    }
                    qFormula.Dequeue();
                    String []f=temp.ToArray();
                    result.Enqueue(simplyFormula(f));
                }
                 else if (!(qFormula.Peek().Equals("AND") || qFormula.Peek().Equals("and")) &&
                          !(qFormula.Peek().Equals("OR") || qFormula.Peek().Equals("or")))
                 {
                     if (qFormula.Peek().Contains(">") || qFormula.Peek().Contains("<") || qFormula.Peek().Contains("="))
                     {
                         ParserB p = new ParserB(qFormula.Dequeue(), stock);
                         try
                         {
                             result.Enqueue(p.getResult().ToString());
                         }
                         catch (Exception e)
                         {
                             return e.Message;
                         }
                     }
                     else
                     {
                         ParserD p = new ParserD(qFormula.Dequeue(), stock);
                         try
                         {
                             result.Enqueue(p.getResult().ToString());
                         }
                         catch(Exception e)
                         {
                             return e.Message;
                         }
                     }
                }
               
                else {
                    result.Enqueue(qFormula.Dequeue());
                }
            }
            Stack<String> sResult = new Stack<string>();
            Stack<String> sTemp = new Stack<string>();
            while (result.Count() != 0) {
                sTemp.Push(result.Dequeue());
            }
            while (sTemp.Count() != 0) {
                sResult.Push(sTemp.Pop());
            }
            while (sResult.Count() > 1) {
                Boolean o1;
                try{
                    o1 = Convert.ToBoolean(sResult.Pop());
                }
                catch(FormatException e)
                {
                    return "表达式错误！请检查AND(OR)运算符操作数据是否为布尔值！";
                }
                catch
                {
                    return "表达式错误！请检查AND(OR)运算符前是否缺失数据！";
                }

                String opreator = sResult.Pop();
                Boolean o2;
                try
                {
                    o2 = Convert.ToBoolean(sResult.Pop());
                }
                catch (FormatException e)
                {
                    return "表达式错误！请检查AND(OR)运算符操作数据是否为布尔值！";
                }
                catch
                {
                    return "表达式错误！请检查AND(OR)运算符后是否缺失数据！";
                }
                
                if (opreator.Equals("AND") || opreator.Equals("and")) 
                {
                    String tempResult = (o1 && o2).ToString();
                    sResult.Push(tempResult);
                }
                if (opreator.Equals("OR") || opreator.Equals("or"))
                {
                    String tempResult = (o1 || o2).ToString();
                    sResult.Push(tempResult);
                }
            }

            return sResult.Pop();
        }
    }
}
