namespace FormulaEditor
{
    partial class FormulaEdit
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.calculate = new System.Windows.Forms.Button();
            this.formula = new System.Windows.Forms.TextBox();
            this.formulaListBox = new System.Windows.Forms.ListBox();
            this.result = new System.Windows.Forms.RichTextBox();
            this.tip = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.formulaTitle = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // calculate
            // 
            this.calculate.Location = new System.Drawing.Point(422, 48);
            this.calculate.Name = "calculate";
            this.calculate.Size = new System.Drawing.Size(100, 23);
            this.calculate.TabIndex = 0;
            this.calculate.Text = "计算";
            this.calculate.UseVisualStyleBackColor = true;
            this.calculate.Click += new System.EventHandler(this.calculate_Click);
            // 
            // formula
            // 
            this.formula.Location = new System.Drawing.Point(44, 50);
            this.formula.MaxLength = 10000;
            this.formula.Name = "formula";
            this.formula.Size = new System.Drawing.Size(359, 21);
            this.formula.TabIndex = 1;
            // 
            // formulaListBox
            // 
            this.formulaListBox.FormattingEnabled = true;
            this.formulaListBox.ItemHeight = 12;
            this.formulaListBox.Location = new System.Drawing.Point(422, 86);
            this.formulaListBox.Name = "formulaListBox";
            this.formulaListBox.Size = new System.Drawing.Size(100, 220);
            this.formulaListBox.TabIndex = 2;
            this.formulaListBox.SelectedIndexChanged += new System.EventHandler(this.formulaListBox_SelectedIndexChanged);
            // 
            // result
            // 
            this.result.Location = new System.Drawing.Point(44, 86);
            this.result.Name = "result";
            this.result.Size = new System.Drawing.Size(359, 220);
            this.result.TabIndex = 3;
            this.result.Text = "";
            // 
            // tip
            // 
            this.tip.AutoSize = true;
            this.tip.Location = new System.Drawing.Point(42, 312);
            this.tip.Name = "tip";
            this.tip.Size = new System.Drawing.Size(41, 12);
            this.tip.TabIndex = 4;
            this.tip.Text = "示例：";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(422, 19);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 23);
            this.save.TabIndex = 5;
            this.save.Text = "保存";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // formulaTitle
            // 
            this.formulaTitle.Location = new System.Drawing.Point(44, 19);
            this.formulaTitle.MaxLength = 80;
            this.formulaTitle.Name = "formulaTitle";
            this.formulaTitle.Size = new System.Drawing.Size(359, 21);
            this.formulaTitle.TabIndex = 6;
            // 
            // FormulaEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 345);
            this.Controls.Add(this.formulaTitle);
            this.Controls.Add(this.save);
            this.Controls.Add(this.tip);
            this.Controls.Add(this.result);
            this.Controls.Add(this.formulaListBox);
            this.Controls.Add(this.formula);
            this.Controls.Add(this.calculate);
            this.Name = "FormulaEdit";
            this.Text = "公式编辑器";
            this.Load += new System.EventHandler(this.FormulaEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button calculate;
        private System.Windows.Forms.TextBox formula;
        private System.Windows.Forms.ListBox formulaListBox;
        private System.Windows.Forms.RichTextBox result;
        private System.Windows.Forms.Label tip;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.TextBox formulaTitle;
    }
}

