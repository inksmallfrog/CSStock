using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockInfo;
using System.Data.SqlClient;

namespace FormulaEditor
{
    public struct Formula{
        public string title;
        public string formula;
        public override string ToString()
        {
            return title;
        }
    }

    public partial class FormulaEdit : Form
    {
        private IStock stock;
        private string tipStr;
        private SqlConnection con;
        private string formulaTB;
        private List<Formula> formulaList = new List<Formula>();

        public FormulaEdit(IStock _stock)
        {
            InitializeComponent();
            stock = _stock;
            tipStr = "说明：\n" +
                    "支持运算符：\n" +
                    "  运算类：+ - * / () [] pow sin cos tan asin acos atan\n" +
                    "\t  exp ln log sqrt abs ceiling floor intpart\n" +
                    "\t  rand sign round fracpart max min mod\n" +
                    "\t  多参数函数请用[arg0,arg1,…]予以标示\n" +
                    "  布尔类：if nif and or\n" +
                    "\t  布尔运算请用{}予以标示\n\n" +
                    "支持数据类型：\n" +
                    "  当日数据：open close high low vol";
        }

        private void FormulaEdit_Load(object sender, EventArgs e)
        {
            InitFormulaTitleBox();
            InitFormulaBox();
            InitResult();
            InitFormulaList();
            tip.Text = "示例：{(pow[high,pow[2,1]]*3+low)*(5+3)>2+4/(high+3)}AND{5>3}";
        }

        private void InitFormulaTitleBox()
        {
            formulaTitle.Text = "请输入公式名";
            formulaTitle.Enter += new System.EventHandler(formulaTitle_Enter);
            formulaTitle.Leave += new System.EventHandler(formulaTitle_Leave);
        }

        private void formulaTitle_Enter(object sender, EventArgs e)
        {
            if (formulaTitle.Text == "请输入公式名")
            {
                formulaTitle.Text = "";
            }
        }

        private void formulaTitle_Leave(object sender, EventArgs e)
        {
            if (formulaTitle.Text == "")
            {
                formulaTitle.Text = "请输入公式名";
            }
        }

        private void InitFormulaBox()
        {
            formula.Text = "请输入公式";
            formula.Enter += new System.EventHandler(formula_Enter);
            formula.Leave += new System.EventHandler(formula_Leave);
        }

        private void formula_Enter(object sender, EventArgs e)
        {
            if (formula.Text == "请输入公式")
            {
                formula.Text = "";
            }
        }

        private void formula_Leave(object sender, EventArgs e)
        {
            if (formula.Text == "")
            {
                formula.Text = "请输入公式";
            }
        }

        private void InitResult()
        {
            result.Text = tipStr;
        }

        private void InitFormulaList()
        {
            formulaTB = "formulaTB";

            con = new SqlConnection("Integrated Security=SSPI;Initial Catalog=stock;Data Source=localhost;");
            con.Open();
            InitDatabase();
            LoadFormulaList();
            BindFormulaList();
        }

        private void InitDatabase()
        {
            string sql;
            SqlCommand cmd = new SqlCommand();

            sql = "if not exists (select * from sysobjects where id = object_id(N'" + formulaTB + "') and OBJECTPROPERTY(id, N'IsUserTable') = 1)\n" + 
                  "create table " + formulaTB +
                  "(formulaTitle CHAR(80), formula CHAR(1000));";
            cmd.Connection = con;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private void LoadFormulaList()
        {
            string sql;
            SqlCommand cmd = new SqlCommand();
            sql = "select * from " + formulaTB + ";";
            cmd.Connection = con;
            cmd.CommandText = sql;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Formula f = new Formula();
                f.title = (string)reader["formulaTitle"];
                f.formula = (string)reader["formula"];
                formulaList.Add(f);
            }
            reader.Close();
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            FormulaGetter getter = new FormulaGetter(formula.Text, stock);
            result.Text = getter.getResult() + "\n\n=====================================\n" + tipStr;
        }

        private void BindFormulaList()
        {
            formulaListBox.SelectedIndexChanged -= formulaListBox_SelectedIndexChanged;
            formulaListBox.DataSource = null;
            formulaListBox.DataSource = formulaList;
            formulaListBox.SelectedIndex = -1;
            formulaListBox.SelectedIndexChanged += formulaListBox_SelectedIndexChanged;
        }

        private void formulaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            formulaTitle.Text = ((Formula)formulaListBox.SelectedItem).title;
            formula.Text = ((Formula)formulaListBox.SelectedItem).formula;

            FormulaGetter getter = new FormulaGetter(formula.Text, stock);
            result.Text = getter.getResult() + "\n\n=====================================\n" + tipStr;
        }

        private void save_Click(object sender, EventArgs e)
        {
            FormulaGetter getter = new FormulaGetter(formula.Text, stock);
            if (getter.getResult().Contains("表达式错误！"))
            {
                result.Text = "公式错误！禁止保存错误的公式，请检查并修改！\n" + "\n\n=====================================\n" + tipStr;
            }
            else
            {
                SaveFormulaToList();
                SaveFormulaToDB();
                result.Text = "保存成功！\n" + "\n\n=====================================\n" + tipStr;
                BindFormulaList();
            }
        }
        private void SaveFormulaToList()
        {
            Formula f = new Formula();
            f.title = formulaTitle.Text;
            f.formula = formula.Text;
            for (int i = 0; i < formulaList.Count; ++i )
            {
                if (formulaList[i].title.Equals(f.title))
                {
                    formulaList[i] = f;
                    return;
                }
            }
            formulaList.Add(f);
        }
        private void SaveFormulaToDB()
        {
            string sql;
            SqlCommand cmd = new SqlCommand();
            sql = "select * from " + formulaTB + " where formulaTitle='" + formulaTitle.Text + "';";
            cmd.Connection = con;
            cmd.CommandText = sql;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sql = "update " + formulaTB + " set formula='" + formula.Text + "' where formulaTitle=" + formulaTitle.Text + ";";
            }
            else
            {
                sql = "insert into " + formulaTB + "(formulaTitle, formula) values('" + formulaTitle.Text + "', '" + formula.Text + "');";
            }
            reader.Close();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
