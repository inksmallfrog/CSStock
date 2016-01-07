namespace StockMonitor.Forms
{
    partial class NewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stkCode = new System.Windows.Forms.TextBox();
            this.stkName = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "股市代码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "股市名称";
            // 
            // stkCode
            // 
            this.stkCode.Location = new System.Drawing.Point(225, 78);
            this.stkCode.Name = "stkCode";
            this.stkCode.Size = new System.Drawing.Size(208, 21);
            this.stkCode.TabIndex = 2;
            // 
            // stkName
            // 
            this.stkName.Location = new System.Drawing.Point(225, 120);
            this.stkName.Name = "stkName";
            this.stkName.Size = new System.Drawing.Size(208, 21);
            this.stkName.TabIndex = 3;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(225, 189);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 4;
            this.createButton.Text = "打开";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(110, 163);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 12);
            this.errorLabel.TabIndex = 5;
            // 
            // newForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 269);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.stkName);
            this.Controls.Add(this.stkCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "newForm";
            this.Text = "新窗口";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox stkCode;
        private System.Windows.Forms.TextBox stkName;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Label errorLabel;
    }
}