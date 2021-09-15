
namespace Dt
{
    partial class InsertWinForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertWinForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cls = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._cb = new System.Windows.Forms.ComboBox();
            this._lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "命名空间";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _ns
            // 
            this._ns.Location = new System.Drawing.Point(187, 7);
            this._ns.Name = "_ns";
            this._ns.Size = new System.Drawing.Size(269, 21);
            this._ns.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "类名";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cls
            // 
            this._cls.Location = new System.Drawing.Point(187, 27);
            this._cls.Name = "_cls";
            this._cls.Size = new System.Drawing.Size(269, 21);
            this._cls.TabIndex = 19;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 201);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 22;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 24;
            this.label3.Text = "窗口布局";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cb
            // 
            this._cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cb.FormattingEnabled = true;
            this._cb.Items.AddRange(new object[] {
            "空白窗口",
            "主区窗口",
            "动态主区窗口",
            "两区窗口",
            "三区窗口"});
            this._cb.Location = new System.Drawing.Point(187, 47);
            this._cb.Name = "_cb";
            this._cb.Size = new System.Drawing.Size(269, 20);
            this._cb.TabIndex = 25;
            this._cb.SelectedIndexChanged += new System.EventHandler(this._cb_SelectedIndexChanged);
            // 
            // _lbl
            // 
            this._lbl.ForeColor = System.Drawing.Color.Black;
            this._lbl.Location = new System.Drawing.Point(12, 81);
            this._lbl.Name = "_lbl";
            this._lbl.Size = new System.Drawing.Size(444, 88);
            this._lbl.TabIndex = 26;
            this._lbl.Text = "label4";
            // 
            // InsertWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 240);
            this.Controls.Add(this._lbl);
            this.Controls.Add(this._cb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._cls);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsertWinForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加窗口";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _cls;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _cb;
        private System.Windows.Forms.Label _lbl;
    }
}