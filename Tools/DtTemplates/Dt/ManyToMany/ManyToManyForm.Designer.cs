
namespace Dt.ManyToMany
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label10 = new System.Windows.Forms.Label();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this._cbSql = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
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
            this.label1.Location = new System.Drawing.Point(12, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsa
            // 
            this._clsa.Location = new System.Drawing.Point(187, 124);
            this._clsa.Name = "_clsa";
            this._clsa.Size = new System.Drawing.Size(269, 21);
            this._clsa.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "主实体标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsaTitle
            // 
            this._clsaTitle.Location = new System.Drawing.Point(187, 144);
            this._clsaTitle.Name = "_clsaTitle";
            this._clsaTitle.Size = new System.Drawing.Size(269, 21);
            this._clsaTitle.TabIndex = 13;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 329);
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
            this._info.Location = new System.Drawing.Point(12, 284);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(444, 35);
            this._info.TabIndex = 23;
            this._info.Text = " 本框架只生成主实体类，不生成关联的实体类，关联实体类在生成代码中已注释掉";
            // 
            // _clsb
            // 
            this._clsb.Location = new System.Drawing.Point(187, 184);
            this._clsb.Name = "_clsb";
            this._clsb.Size = new System.Drawing.Size(269, 21);
            this._clsb.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 24;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsbTitle
            // 
            this._clsbTitle.Location = new System.Drawing.Point(187, 204);
            this._clsbTitle.Name = "_clsbTitle";
            this._clsbTitle.Size = new System.Drawing.Size(269, 21);
            this._clsbTitle.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 204);
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
            this._cbSearch.Location = new System.Drawing.Point(187, 243);
            this._cbSearch.Name = "_cbSearch";
            this._cbSearch.Size = new System.Drawing.Size(269, 20);
            this._cbSearch.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 243);
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
            this._agentName.Text = "AtSvc";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(12, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 21);
            this.label8.TabIndex = 45;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(14, 31);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(65, 12);
            this.linkLabel1.TabIndex = 104;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "服务代理类";
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(12, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(444, 21);
            this.label10.TabIndex = 115;
            this.label10.Text = "表与实体映射";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(14, 90);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 120;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(187, 85);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(269, 21);
            this._svcUrl.TabIndex = 118;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(12, 85);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(176, 21);
            this.label11.TabIndex = 119;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(14, 110);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(29, 12);
            this.linkLabel4.TabIndex = 123;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "主表";
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(187, 105);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(269, 20);
            this._cbTbls.TabIndex = 121;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            this._cbTbls.SelectionChangeCommitted += new System.EventHandler(this._cbTbls_SelectionChangeCommitted);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(12, 105);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(176, 21);
            this.label9.TabIndex = 122;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(14, 129);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(65, 12);
            this.linkLabel2.TabIndex = 124;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "主实体名称";
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Location = new System.Drawing.Point(14, 188);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(161, 12);
            this.linkLabel5.TabIndex = 125;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "关联实体名称(逗号隔开多个)";
            // 
            // linkLabel7
            // 
            this.linkLabel7.AutoSize = true;
            this.linkLabel7.Location = new System.Drawing.Point(14, 169);
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.Size = new System.Drawing.Size(107, 12);
            this.linkLabel7.TabIndex = 136;
            this.linkLabel7.TabStop = true;
            this.linkLabel7.Text = "自动生成主实体sql";
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Location = new System.Drawing.Point(12, 164);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(176, 21);
            this.label13.TabIndex = 134;
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbSql
            // 
            this._cbSql.AutoSize = true;
            this._cbSql.Checked = true;
            this._cbSql.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbSql.Location = new System.Drawing.Point(257, 167);
            this._cbSql.Name = "_cbSql";
            this._cbSql.Size = new System.Drawing.Size(15, 14);
            this._cbSql.TabIndex = 133;
            this._cbSql.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Location = new System.Drawing.Point(187, 164);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(269, 21);
            this.label14.TabIndex = 135;
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ManyToManyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 362);
            this.Controls.Add(this.linkLabel7);
            this.Controls.Add(this.label13);
            this.Controls.Add(this._cbSql);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.linkLabel5);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.linkLabel1);
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
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox _cbSql;
        private System.Windows.Forms.Label label14;
    }
}