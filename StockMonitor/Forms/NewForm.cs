using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace StockMonitor.Forms
{
    public partial class NewForm : Form
    {
        public NewForm()
        {
            InitializeComponent();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            p.StartInfo.Arguments = stkCode.Text + " " + stkName.Text;
            p.Start();
            this.Hide();
        }
    }
}
