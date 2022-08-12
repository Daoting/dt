
namespace Dt.OnToMany
{
    partial class OnToManyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnToManyForm));
            this.label2 = new System.Windows.Forms.Label();
            this._nameSpace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._clsa = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._clsaTitle = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._info = new System.Windows.Forms.Label();
            this._cbSearch = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this._clsbTitle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._clsb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._cbWin = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this._agentName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label12 = new System.Windows.Forms.Label();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this._childTbls = new System.Windows.Forms.TextBox();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this._cbSql = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
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
            // _nameSpace
            // 
            this._nameSpace.Location = new System.Drawing.Point(187, 7);
            this._nameSpace.Name = "_nameSpace";
            this._nameSpace.Size = new System.Drawing.Size(269, 21);
            this._nameSpace.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsa
            // 
            this._clsa.Location = new System.Drawing.Point(187, 127);
            this._clsa.Name = "_clsa";
            this._clsa.Size = new System.Drawing.Size(269, 21);
            this._clsa.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "父实体标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsaTitle
            // 
            this._clsaTitle.Location = new System.Drawing.Point(187, 147);
            this._clsaTitle.Name = "_clsaTitle";
            this._clsaTitle.Size = new System.Drawing.Size(269, 21);
            this._clsaTitle.TabIndex = 13;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 348);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 22;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _info
            // 
            this._info.AutoSize = true;
            this._info.ForeColor = System.Drawing.Color.Black;
            this._info.Location = new System.Drawing.Point(12, 327);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(137, 12);
            this._info.TabIndex = 23;
            this._info.Text = "自动生成所有父子类实体";
            // 
            // _cbSearch
            // 
            this._cbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSearch.FormattingEnabled = true;
            this._cbSearch.Items.AddRange(new object[] {
            "通用搜索面板",
            "自定义搜索面板"});
            this._cbSearch.Location = new System.Drawing.Point(187, 269);
            this._cbSearch.Name = "_cbSearch";
            this._cbSearch.Size = new System.Drawing.Size(269, 20);
            this._cbSearch.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 269);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 21);
            this.label6.TabIndex = 39;
            this.label6.Text = "父实体列表搜索面板";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsbTitle
            // 
            this._clsbTitle.Location = new System.Drawing.Point(187, 207);
            this._clsbTitle.Name = "_clsbTitle";
            this._clsbTitle.Size = new System.Drawing.Size(269, 21);
            this._clsbTitle.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 207);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 21);
            this.label5.TabIndex = 38;
            this.label5.Text = "子实体标题(逗号隔开多个)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsb
            // 
            this._clsb.Location = new System.Drawing.Point(187, 187);
            this._clsb.Name = "_clsb";
            this._clsb.Size = new System.Drawing.Size(269, 21);
            this._clsb.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 37;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbWin
            // 
            this._cbWin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbWin.FormattingEnabled = true;
            this._cbWin.Items.AddRange(new object[] {
            "窗口三栏，左侧父列表，主区父表单，右侧子列表",
            "窗口两栏，左侧父列表，右侧子列表"});
            this._cbWin.Location = new System.Drawing.Point(187, 288);
            this._cbWin.Name = "_cbWin";
            this._cbWin.Size = new System.Drawing.Size(269, 20);
            this._cbWin.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(12, 288);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(176, 21);
            this.label7.TabIndex = 41;
            this.label7.Text = "窗口布局";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.label8.TabIndex = 43;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(15, 31);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(65, 12);
            this.linkLabel1.TabIndex = 103;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "服务代理类";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(14, 112);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(29, 12);
            this.linkLabel4.TabIndex = 118;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "父表";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(14, 92);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 117;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(187, 107);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(269, 20);
            this._cbTbls.TabIndex = 115;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(12, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(176, 21);
            this.label9.TabIndex = 116;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(12, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(444, 21);
            this.label10.TabIndex = 114;
            this.label10.Text = "表与实体映射";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(187, 87);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(269, 21);
            this._svcUrl.TabIndex = 112;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(12, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(176, 21);
            this.label11.TabIndex = 113;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(14, 131);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(65, 12);
            this.linkLabel2.TabIndex = 119;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "父实体名称";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Location = new System.Drawing.Point(12, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(176, 21);
            this.label12.TabIndex = 120;
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Location = new System.Drawing.Point(14, 172);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(29, 12);
            this.linkLabel5.TabIndex = 121;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "子表";
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            // 
            // _childTbls
            // 
            this._childTbls.Location = new System.Drawing.Point(187, 167);
            this._childTbls.Name = "_childTbls";
            this._childTbls.ReadOnly = true;
            this._childTbls.Size = new System.Drawing.Size(269, 21);
            this._childTbls.TabIndex = 122;
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.Location = new System.Drawing.Point(14, 191);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(149, 12);
            this.linkLabel6.TabIndex = 123;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "子实体名称(逗号隔开多个)";
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // linkLabel7
            // 
            this.linkLabel7.AutoSize = true;
            this.linkLabel7.Location = new System.Drawing.Point(14, 232);
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.Size = new System.Drawing.Size(71, 12);
            this.linkLabel7.TabIndex = 127;
            this.linkLabel7.TabStop = true;
            this.linkLabel7.Text = "自动生成sql";
            this.linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel7_LinkClicked);
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Location = new System.Drawing.Point(12, 227);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(176, 21);
            this.label13.TabIndex = 125;
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbSql
            // 
            this._cbSql.AutoSize = true;
            this._cbSql.Checked = true;
            this._cbSql.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbSql.Location = new System.Drawing.Point(257, 230);
            this._cbSql.Name = "_cbSql";
            this._cbSql.Size = new System.Drawing.Size(15, 14);
            this._cbSql.TabIndex = 124;
            this._cbSql.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Location = new System.Drawing.Point(187, 227);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(269, 21);
            this.label14.TabIndex = 126;
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(424, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(31, 23);
            this.button1.TabIndex = 128;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // OnToManyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 384);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkLabel7);
            this.Controls.Add(this.label13);
            this.Controls.Add(this._cbSql);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this._childTbls);
            this.Controls.Add(this.linkLabel5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this._agentName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._cbWin);
            this.Controls.Add(this.label7);
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
            this.Controls.Add(this._nameSpace);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OnToManyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加一对多框架文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _nameSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _clsa;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _clsaTitle;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label _info;
        private System.Windows.Forms.ComboBox _cbSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _clsbTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _clsb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _cbWin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _agentName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.TextBox _childTbls;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox _cbSql;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button1;
    }
}