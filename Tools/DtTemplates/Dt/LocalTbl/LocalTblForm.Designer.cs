
namespace Dt.LocalTbl
{
    partial class LocalTblForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalTblForm));
            this.label2 = new System.Windows.Forms.Label();
            this._nameSpace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._entityName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._entityTitle = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._info = new System.Windows.Forms.Label();
            this._cbSearch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._cbWin = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this._dbPath = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
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
            this.label1.Location = new System.Drawing.Point(12, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _entityName
            // 
            this._entityName.Location = new System.Drawing.Point(187, 107);
            this._entityName.Name = "_entityName";
            this._entityName.ReadOnly = true;
            this._entityName.Size = new System.Drawing.Size(269, 21);
            this._entityName.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "实体中文标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _entityTitle
            // 
            this._entityTitle.Location = new System.Drawing.Point(187, 127);
            this._entityTitle.Name = "_entityTitle";
            this._entityTitle.Size = new System.Drawing.Size(269, 21);
            this._entityTitle.TabIndex = 13;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 242);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 100;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _info
            // 
            this._info.AutoSize = true;
            this._info.ForeColor = System.Drawing.Color.Black;
            this._info.Location = new System.Drawing.Point(10, 222);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(233, 12);
            this._info.TabIndex = 23;
            this._info.Text = "实现本地sqlite库单表增删改查(CRUD)功能";
            // 
            // _cbSearch
            // 
            this._cbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSearch.FormattingEnabled = true;
            this._cbSearch.Items.AddRange(new object[] {
            "通用搜索面板",
            "自定义搜索面板"});
            this._cbSearch.Location = new System.Drawing.Point(187, 166);
            this._cbSearch.Name = "_cbSearch";
            this._cbSearch.Size = new System.Drawing.Size(269, 20);
            this._cbSearch.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 26;
            this.label4.Text = "搜索面板";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbWin
            // 
            this._cbWin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbWin.FormattingEnabled = true;
            this._cbWin.Items.AddRange(new object[] {
            "窗口三栏，左侧搜索栏",
            "窗口两栏，左侧列表栏，搜索合并到列表栏"});
            this._cbWin.Location = new System.Drawing.Point(187, 186);
            this._cbWin.Name = "_cbWin";
            this._cbWin.Size = new System.Drawing.Size(269, 20);
            this._cbWin.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 186);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 21);
            this.label5.TabIndex = 28;
            this.label5.Text = "窗口布局";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _dbPath
            // 
            this._dbPath.Location = new System.Drawing.Point(187, 67);
            this._dbPath.Name = "_dbPath";
            this._dbPath.ReadOnly = true;
            this._dbPath.Size = new System.Drawing.Size(269, 21);
            this._dbPath.TabIndex = 11;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(14, 71);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(101, 12);
            this.linkLabel1.TabIndex = 102;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "选择sqlite库文件";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 21);
            this.label6.TabIndex = 30;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(14, 111);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(53, 12);
            this.linkLabel2.TabIndex = 103;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "实体类名";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(12, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(444, 21);
            this.label8.TabIndex = 107;
            this.label8.Text = "表与实体映射";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(187, 87);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(269, 20);
            this._cbTbls.TabIndex = 108;
            this._cbTbls.SelectedIndexChanged += new System.EventHandler(this._cbTbls_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(12, 87);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(176, 21);
            this.label9.TabIndex = 109;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(14, 92);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 111;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(412, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 21);
            this.button1.TabIndex = 116;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LocalTblForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 278);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this._dbPath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._cbWin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._cbSearch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._info);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._entityTitle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._entityName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._nameSpace);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocalTblForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加本地库单表框架文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _nameSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _entityName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _entityTitle;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label _info;
        private System.Windows.Forms.ComboBox _cbSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _cbWin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _dbPath;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Button button1;
    }
}