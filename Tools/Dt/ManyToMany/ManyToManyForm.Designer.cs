
namespace Dt
{
    partial class ManyToManyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManyToManyForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._clsa = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._clsaTitle = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._info = new System.Windows.Forms.Label();
            this._clsb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._clsbTitle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._cbSearch = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this._agentName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
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
            this._ns.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "主实体类名";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsa
            // 
            this._clsa.Location = new System.Drawing.Point(187, 47);
            this._clsa.Name = "_clsa";
            this._clsa.Size = new System.Drawing.Size(269, 21);
            this._clsa.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "主实体标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsaTitle
            // 
            this._clsaTitle.Location = new System.Drawing.Point(187, 67);
            this._clsaTitle.Name = "_clsaTitle";
            this._clsaTitle.Size = new System.Drawing.Size(269, 21);
            this._clsaTitle.TabIndex = 13;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 249);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 32;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _info
            // 
            this._info.ForeColor = System.Drawing.Color.Black;
            this._info.Location = new System.Drawing.Point(12, 168);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(444, 40);
            this._info.TabIndex = 23;
            this._info.Text = " 只生成主实体类，关联的实体类在它作为主实体时生成";
            // 
            // _clsb
            // 
            this._clsb.Location = new System.Drawing.Point(187, 87);
            this._clsb.Name = "_clsb";
            this._clsb.Size = new System.Drawing.Size(269, 21);
            this._clsb.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 24;
            this.label4.Text = "关联实体类名(逗号隔开多个)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsbTitle
            // 
            this._clsbTitle.Location = new System.Drawing.Point(187, 107);
            this._clsbTitle.Name = "_clsbTitle";
            this._clsbTitle.Size = new System.Drawing.Size(269, 21);
            this._clsbTitle.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 21);
            this.label5.TabIndex = 26;
            this.label5.Text = "关联实体标题(逗号隔开多个)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbSearch
            // 
            this._cbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSearch.FormattingEnabled = true;
            this._cbSearch.Items.AddRange(new object[] {
            "通用搜索面板",
            "自定义搜索面板"});
            this._cbSearch.Location = new System.Drawing.Point(187, 127);
            this._cbSearch.Name = "_cbSearch";
            this._cbSearch.Size = new System.Drawing.Size(269, 20);
            this._cbSearch.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 21);
            this.label6.TabIndex = 33;
            this.label6.Text = "主实体列表搜索面板";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _agentName
            // 
            this._agentName.Location = new System.Drawing.Point(187, 27);
            this._agentName.Name = "_agentName";
            this._agentName.Size = new System.Drawing.Size(269, 21);
            this._agentName.TabIndex = 11;
            this._agentName.Text = "AtLob";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(12, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 21);
            this.label8.TabIndex = 45;
            this.label8.Text = "代理服务";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ManyToManyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 281);
            this.Controls.Add(this._agentName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._cbSearch);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._clsbTitle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._clsb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._info);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._clsaTitle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._clsa);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManyToManyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加多对多框架文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _clsa;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _clsaTitle;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label _info;
        private System.Windows.Forms.TextBox _clsb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _clsbTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _cbSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _agentName;
        private System.Windows.Forms.Label label8;
    }
}