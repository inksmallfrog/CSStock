using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using StockInfo;

namespace FormulaEditor
{
    class ParserB {
        private String[] m_formula;
        private IStock stock;
        public ParserB(String formula, IStock _stock) {
            stock = _stock;
            Regex r = new Regex("([<>=])");
            m_formula = r.Split(formula);
            int count = 0;
            foreach (String s in m_formula) {
                if (s == "") {
                    count++;
                }
            }
            String[] temp = new String[m_formula.Length - count];
            int countOfTemp = 0;
            for (int i = 0; i < m_formula.Length; i++) {
                if (m_formula[i] != "") {
                    temp[countOfTemp] = m_formula[i];
                    countOfTemp++;
                }
            }
            m_formula = temp;
            //test
            //foreach(string i in m_formula)
            //Console.WriteLine(i);
        }
        public Boolean getResult() {
            ParserD p=new ParserD(m_formula[0], stock);
            decimal o1;
            try
            {
                o1 = p.getResult();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            p=new ParserD(m_formula[2], stock);
            decimal o2;
            try
            {
                o2 = p.getResult();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (m_formula[1].Equals("=")) {
                if (o1 == o2)
                    return true;
                else 
                    return false;
            }
            else if (m_formula[1].Equals(">")) {
                if (o1 > o2)
                    return true;
                else
                    return false;
            }
            else if (m_formula[1].Equals("<")) {
                if (o1 < o2)
                    return true;
                else
                    return false;
            }
            else { return false; }
        }
    }
}
